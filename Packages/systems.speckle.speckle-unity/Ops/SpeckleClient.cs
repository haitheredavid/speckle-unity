using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Speckle.Core.Api;
using Speckle.Core.Kits;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Speckle.ConnectorUnity
{
	// BUG: issue with refreshing object data to editor, probably something with serializing the branch or commit data  
	public abstract class SpeckleClient : MonoBehaviour, IProgress<float>
	{
		protected const string HostApp = HostApplications.Unity.Name;

		[SerializeField] protected GameObject root;
		[SerializeField] protected SpeckleStream stream;
		[SerializeField] protected List<ConverterUnity> converters;

		[SerializeField] private bool expired;

		[SerializeField] private int branchIndex;
		[SerializeField] private int converterIndex;

		[SerializeField] protected float progressAmount;

		protected Client client;
		protected bool isCanceled;

		public Action<int> onTotalChildrenCountKnown;
		public Action<string, Exception> onErrorReport;
		public Action<ConcurrentDictionary<string, int>> onProgressReport;

		public int totalChildCount { get; protected set; }

		public bool isWorking { get; protected set; }

		public List<Branch> Branches { get; protected set; }

		public List<ConverterUnity> Converters
		{
			get => converters.Valid() ? converters : new List<ConverterUnity>();
		}

		protected ConverterUnity converter
		{
			get => converters.Valid(converterIndex) ? converters[converterIndex] : null;

		}

		public Branch activeBranch
		{
			get => Branches.Valid(branchIndex) ? Branches[branchIndex] : null;
		}

		protected virtual void OnEnable()
		{
			// TODO: during the build process this should compile and store these objects. 
			#if UNITY_EDITOR
			converters = GetAllInstances<ConverterUnity>();
			#endif
		}

		private void OnDisable()
		{
			CleanUp();
		}

		private void OnDestroy()
		{
			CleanUp();
		}

		public void Report(float value)
		{
			progressAmount = value;
		}

		protected void Refresh()
		{
			onRepaint?.Invoke();
		}

		public event Action onRepaint;

		public virtual void SetBranch(int i)
		{
			branchIndex = Branches.Check(i);
		}

		public void SetConverter(int i)
		{
			converterIndex = converters.Check(i);
		}

		/// <param name="rootStream">root stream object to use, will default to editor field</param>
		/// <param name="onProgressAction">Action to run when there is download/conversion progress</param>
		/// <param name="onErrorAction">Action to run on error</param>
		public async UniTask<bool> Init(
			SpeckleStream rootStream,
			Action<ConcurrentDictionary<string, int>> onProgressAction = null,
			Action<string, Exception> onErrorAction = null
		)
		{
			onErrorReport = onErrorAction;
			onProgressReport = onProgressAction;

			stream = rootStream;
			if (stream == null || !stream.IsValid())
			{
				ConnectorConsole.Log("Speckle stream object is not setup correctly");
				return false;
			}

			await LoadStream();

			SetSubscriptions();

			onRepaint?.Invoke();

			return client != null;
		}

		protected virtual async UniTask LoadStream()
		{
			var account = await stream.GetAccount();
			client = new Client(account);

			Branches = await client.StreamGetBranches(this.GetCancellationTokenOnDestroy(), stream.Id);
		}

		protected virtual void SetSubscriptions()
		{
			if (client == null) ConnectorConsole.Log($"No active client on {name} to read from");
		}

		protected bool IsReady()
		{
			var res = true;

			if (stream == null || !stream.IsValid())
			{
				ConnectorConsole.Log($"No active stream ready for {name} to use");
				res = false;
			}

			if (client == null)
			{
				ConnectorConsole.Log($"No active client for {name} to use");
				res = false;
			}

			return res;
		}

		protected virtual void CleanUp()
		{
			client?.Dispose();
		}

		#if UNITY_EDITOR
		public static List<T> GetAllInstances<T>() where T : ScriptableObject
		{
			var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
			var items = new List<T>();
			foreach (var g in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(g);
				items.Add(AssetDatabase.LoadAssetAtPath<T>(path));
			}
			return items;
		}
		#endif
	}
}