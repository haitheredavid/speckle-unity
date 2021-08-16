using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Sentry;
using Speckle.Core.Api;
using Speckle.Core.Api.SubscriptionModels;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using UnityEngine;
using ViewTo.Objects.Converter.Unity;

namespace ConnectorUnity
{
  /// <summary>
  /// A Speckle Receiver, it's a wrapper around a basic Speckle Client
  /// that handles conversions and subscriptions for you
  /// </summary>
  [ExecuteAlways]
  public class Receiver : MonoBehaviour
  {
    public string streamName = "stream name", streamId = "stream id", branch = "main", commitId = "commit id";
    [ReadOnly] public int totalChildCount;
    [ReadOnly] public bool expired;

    [SerializeField] private Transform dataAnchor;
    [SerializeField] private bool autoUpdate, purgeOnUpdate;

    public Stream Stream { get; private set; }
    private Action<string, Exception> OnErrorAction;
    private Action<int> OnTotalChildrenCountKnown;
    private Action<ConcurrentDictionary<string, int>> OnProgressAction;
    private Action<GameObject> OnGameObjReceivedAction;
    private Action<object> OnObjReceivedAction;

    private Action<Base> ConvertToUnityAction;

    private Client client;
    private ISpeckleConverter converter;

    private void OnEnable()
    {
      ConvertToUnityAction += SendToConverter;
    }

    /// <summary>
    /// Initializes the Receiver manually
    /// </summary>
    /// <param name="shell">Simple wrapper class for passing receiver data</param>
    /// <param name="monoConverter">Converter to use for processing base objects into unity. Defaults to Objects</param>
    /// <param name="account">Account to use, if null the default account will be used</param>
    /// <param name="onGoReceivedAction">Action to run after new data has been received and converted</param>
    /// <param name="onProgressAction">Action to run when there is download/conversion progress</param>
    /// <param name="onErrorAction">Action to run on error</param>
    /// <param name="onTotalChildrenCountKnown">Action to run when the TotalChildrenCount is known</param>
    public void Init(
      StreamShell shell, Account account = null, ISpeckleConverter monoConverter = null,
      Action<GameObject> onGoReceivedAction = null, Action<object> onObjReceivedAction = null, Action<ConcurrentDictionary<string, int>> onProgressAction = null,
      Action<string, Exception> onErrorAction = null, Action<int> onTotalChildrenCountKnown = null
    )
    {
      if (shell == null) return;

      streamName = shell.streamName;
      streamId = shell.streamId;
      branch = shell.branch;
      commitId = shell.commitId;
      autoUpdate = shell.autoReceive;
      purgeOnUpdate = shell.clearOnUpdate;

      if (monoConverter == null)
      {
        Debug.Log("Using default base converter");
        monoConverter = new ConverterUnity();
      }

      converter = monoConverter;

      OnGameObjReceivedAction = onGoReceivedAction;
      onObjReceivedAction = onObjReceivedAction;
      OnErrorAction = onErrorAction;
      OnProgressAction = onProgressAction;
      OnTotalChildrenCountKnown = onTotalChildrenCountKnown;

      if (account == null)
      {
        Debug.Log("Receiver refering to default account");
        account = AccountManager.GetDefaultAccount();
      }

      client = new Client(account);
      if (client == null)
      {
        Debug.LogWarning("Account could not be connected to Speckle Client");
        return;
      }

      client.SubscribeCommitUpdated(streamId, commitId);
      client.OnCommitUpdated += Client_OnCommitUpdated;

      if (autoUpdate)
      {
        client.SubscribeCommitCreated(streamId);
        client.OnCommitCreated += Client_OnCommitCreated;
      }
    }

    /// <summary>
    /// Gets and converts the data of the last commit on the Stream
    /// </summary>
    /// <returns></returns>
    public async Task Receive()
    {
      if (client == null || string.IsNullOrEmpty(streamId))
        throw new Exception("Receiver has not been initialized. Please call Init().");

      Base @base = null;
      try
      {
        Tracker.TrackPageview(Tracker.RECEIVE);

        var mainBranch = await client.BranchGet(streamId, branch, 1);

        if (!mainBranch.commits.items.Any())
          throw new Exception("This branch has no commits");

        var transport = new ServerTransport(client.Account, streamId);

        Commit commit = mainBranch.commits.items.FirstOrDefault(item => item.id == commitId);
        commit ??= mainBranch.commits.items.FirstOrDefault();

        @base = await Operations.Receive(
          commit.referencedObject,
          remoteTransport: transport,
          onErrorAction: OnErrorAction,
          onProgressAction: OnProgressAction,
          onTotalChildrenCountKnown: OnTotalChildrenCountKnown,
          disposeTransports: true
        );
      }
      catch (Exception e)
      {
        Debug.LogException(e);
        throw new SpeckleException(e.Message, e, true, SentryLevel.Error);
      }
      finally
      {
        SendToConverter(@base);
      }
    }

    #region private methods
    protected void Client_OnCommitUpdated(object sender, CommitInfo e)
    {
      if (e.id == commitId && e.streamId == streamId)
      {
        Debug.Log($"Commit({e.id}) has been Updated!\n"
                  + $"author id: {e.authorId}\n"
                  + $"source app: {e.sourceApplication}\n"
                  + $"message: {e.message}");

        expired = true;
        if (autoUpdate)
          Receive();
        else
          Debug.Log($"Receiver is now expired: {DateTime.Now.TimeOfDay}");

      }
    }
    /// <summary>
    /// Fired when a new commit is created on this stream
    /// It receives and converts the objects and then executes the user defined _onCommitCreated action.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Client_OnCommitCreated(object sender, CommitInfo e)
    {
      if (e.branchName == branch)
      {
        Debug.Log($"Commit({e.id}) has been Created!\n"
                  + $"author id: {e.authorId}\n"
                  + $"source app: {e.sourceApplication}\n"
                  + $"message: {e.message}");

        commitId = e.id;
        Receive();
      }
    }

    private void SendToConverter(Base @base)
    {
      if (@base == null) return;

      try
      {
        if (purgeOnUpdate && dataAnchor != null)
          ConnectorUtilities.SafeDestroy(dataAnchor.gameObject);

        var @object = converter.ConvertToNative(@base);

        if (@object is GameObject go)
        {
          // TODO: handle this properly when updating
          dataAnchor = go.transform;
          OnObjReceivedAction?.Invoke(go);
        }
        else
          OnObjReceivedAction?.Invoke(@object);
      }
      catch (Exception e)
      {
        throw new SpeckleException(e.Message, e, true, SentryLevel.Error);
      }
    }

    private void OnDestroy()
    {
      if (dataAnchor != null)
        ConnectorUtilities.SafeDestroy(dataAnchor.gameObject);

      client?.CommitCreatedSubscription?.Dispose();
      client?.CommitUpdatedSubscription?.Dispose();
    }
    #endregion

  }
}