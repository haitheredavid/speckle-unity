using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using UnityEngine;
using UnityEngine.Networking;

namespace Speckle.ConnectorUnity
{
	[CreateAssetMenu(menuName = "Speckle/Create Speckle Stream Object", fileName = "SpeckleStream", order = 0)]
	public class SpeckleStream : ScriptableObject
	{

		private const string PREV = "preview";
		private const string STR = "streams";

		[SerializeField] private string description;
		[SerializeField] private string streamName;
		[SerializeField] private string serverUrl;
		[SerializeField] private string id;
		[SerializeField] private string branchName;
		[SerializeField] private string commitId;
		[SerializeField] private string objectId;

		[SerializeField] private string originalInput;

		[SerializeField] private List<BranchWrapper> _branches;
		[SerializeField] private List<CommitWrapper> _commits;

		private StreamWrapper _wrapper;

		public StreamWrapper Wrapper
		{
			get { return _wrapper ??= new StreamWrapper(originalInput); }
		}

		public string Name
		{
			get => streamName;
		}

		public string Description
		{
			get => description;
		}

		public string ServerUrl
		{
			get => serverUrl;
		}

		public string Id
		{
			get => id;
		}

		public string CommitId
		{
			get => commitId;
		}

		public string BranchName
		{
			get => branchName;
		}

		public string ObjectId
		{
			get => objectId;
		}

		public string OriginalInput
		{
			get => originalInput;
		}

		public StreamWrapperType Type
		{
			get => Wrapper.Type;
		}

		/// <summary>
		/// Initialize a simple stream object that connects the stream wrapper data to the editor  
		/// </summary>
		/// <param name="streamUrlOrId">If set to null will use the editor data</param>
		/// <returns></returns>
		public bool Init(string streamUrlOrId = null)
		{
			if (streamUrlOrId.Valid())
				originalInput = streamUrlOrId;

			_wrapper = new StreamWrapper(originalInput);

			return Setup();
		}

		public async UniTask<bool> TrySetNew(string streamId, string user, string server)
		{
			_wrapper = new StreamWrapper(streamId, user, server);

			if (!Setup())
			{
				Debug.Log("Setup was not done correctly");
				return false;
			}

			var client = new Client(await _wrapper.GetAccount());

			var stream = await client.StreamGet(_wrapper.StreamId);

			if (stream == null)
				return false;

			streamName = stream.name;
			description = stream.description;

			// TODO: probably a better way to set the most active branch... or just main
			if (!branchName.Valid())
			{
				branchName = stream.branches.items.FirstOrDefault().name;
			}

			_branches = new List<BranchWrapper>();

			foreach (var branch in stream.branches.items)
			{
				var b = new BranchWrapper(branch);
				_branches.Add(b);
			}

			return _wrapper.IsValid;
		}

		private bool Setup()
		{
			if (_wrapper is not { IsValid: true })
			{
				ConnectorConsole.Log("Invalid input for stream");
				return false;
			}

			id = _wrapper.StreamId;
			commitId = _wrapper.CommitId;
			objectId = _wrapper.ObjectId;
			serverUrl = _wrapper.ServerUrl;
			branchName = _wrapper.BranchName;
			originalInput = _wrapper.OriginalInput;

			return true;
		}

		public async UniTask<Account> GetAccount()
		{
			return await Wrapper.GetAccount();
		}

		public bool IsValid()
		{
			return Wrapper.IsValid;
		}

		public override string ToString()
		{
			return Wrapper.ToString();
		}

		public string GetUrl(bool isPreview)
		{
			string url;
			switch (Type)
			{
				case StreamWrapperType.Undefined:
					ConnectorConsole.Warn($"{name} is not a valid stream, bailing on the preview thing");
					url = null;
					break;
				case StreamWrapperType.Stream:
					url = $"{Wrapper.ServerUrl}/{(isPreview ? PREV : STR)}/{Wrapper.StreamId}";
					break;
				case StreamWrapperType.Commit:
					url = $"{Wrapper.ServerUrl}/{(isPreview ? PREV : STR)}/{Wrapper.StreamId}/commits/{Wrapper.CommitId}";
					break;
				case StreamWrapperType.Branch:
					url = $"{Wrapper.ServerUrl}/{(isPreview ? PREV : STR)}/{Wrapper.StreamId}/branches/{Wrapper.BranchName}";
					break;
				case StreamWrapperType.Object:
					url = $"{Wrapper.ServerUrl}/{(isPreview ? PREV : STR)}/{Wrapper.StreamId}/objects/{Wrapper.ObjectId}";
					break;
				default:
					url = null;
					break;
			}

			return url;
		}

		public async UniTask<Texture2D> GetPreview()
		{
			string url = GetUrl(true);

			if (!url.Valid())
				return null;

			var www = await UnityWebRequestTexture.GetTexture(url).SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success)
			{
				ConnectorConsole.Warn(www.error);
				return null;
			}
			return DownloadHandlerTexture.GetContent(www);
		}
	}
}