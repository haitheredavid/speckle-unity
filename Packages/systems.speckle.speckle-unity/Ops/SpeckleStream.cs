using Cysharp.Threading.Tasks;
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
		[SerializeField] private string userId;

		[SerializeField] private string originalInput;

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

		public string UserId
		{
			get => userId;
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

		public bool Init(string stream, string user, string server, string title = null, string info = null)
		{
			ConnectorConsole.Log($"Setting new Stream Object with {stream} to user {user} on {server}");

			_wrapper = new StreamWrapper(stream, user, server);
			streamName = title;
			description = info;

			return Setup();
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

			ConnectorConsole.Log($"Setting new Stream with {originalInput}");
			_wrapper = new StreamWrapper(originalInput);

			return Setup();
		}

		private bool Setup()
		{
			if (_wrapper is not { IsValid: true })
			{
				ConnectorConsole.Log("Invalid input for stream");
				return false;
			}

			id = _wrapper.StreamId;
			userId = _wrapper.UserId;
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