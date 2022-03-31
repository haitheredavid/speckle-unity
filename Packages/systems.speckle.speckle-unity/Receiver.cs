using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Objects.Converter.Unity;
using Sentry;
using Speckle.Core.Api;
using Speckle.Core.Api.SubscriptionModels;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Transports;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  /// <summary>
  ///   A Speckle Receiver, it's a wrapper around a basic Speckle Client
  ///   that handles conversions and subscriptions for you
  /// </summary>
  [RequireComponent(typeof(RecursiveConverter))]
  public class Receiver : MonoBehaviour
  {
    [SerializeField] private StreamShell streamShell;

    public int TotalChildrenCount;
    public GameObject ReceivedData;

    private bool AutoReceive;
    private bool DeleteOld;
    private Action<GameObject> OnDataReceivedAction;
    private Action<string, Exception> OnErrorAction;
    private Action<ConcurrentDictionary<string, int>> OnProgressAction;
    private Action<int> OnTotalChildrenCountKnown;

    [SerializeField] private ConverterUnity objectConverter;

    private Client Client { get; set; }

    /// <summary>
    ///   Initializes the Receiver manually
    /// </summary>
    /// <param name="shell">Id of the stream to receive</param>
    /// <param name="autoReceive">If true, it will automatically receive updates sent to this stream</param>
    /// <param name="deleteOld">If true, it will delete previously received objects when new one are received</param>
    /// <param name="account">Account to use, if null the default account will be used</param>
    /// <param name="converter">Object converter to use </param>
    /// <param name="onDataReceivedAction">Action to run after new data has been received and converted</param>
    /// <param name="onProgressAction">Action to run when there is download/conversion progress</param>
    /// <param name="onErrorAction">Action to run on error</param>
    /// <param name="onTotalChildrenCountKnown">Action to run when the TotalChildrenCount is known</param>
    public void Init(
      StreamShell shell, Account account = null, ConverterUnity converter = null, bool autoReceive = false, bool deleteOld = true,
      Action<GameObject> onDataReceivedAction = null, Action<ConcurrentDictionary<string, int>> onProgressAction = null,
      Action<string, Exception> onErrorAction = null, Action<int> onTotalChildrenCountKnown = null
    )
    {
      streamShell = shell;
      AutoReceive = autoReceive;
      DeleteOld = deleteOld;
      OnDataReceivedAction = onDataReceivedAction;
      OnErrorAction = onErrorAction;
      OnProgressAction = onProgressAction;
      OnTotalChildrenCountKnown = onTotalChildrenCountKnown;

      if (this.objectConverter == null)
      {
        this.objectConverter = converter != null ? converter : ScriptableObject.CreateInstance<ConverterUnity>();
      }


      Client = new Client(account ?? AccountManager.GetDefaultAccount());


      if (AutoReceive)
      {
        Client.SubscribeCommitCreated(streamShell.streamId);
        // Client.OnCommitCreated += Client_OnCommitCreated;
      }
    }

    /// <summary>
    ///   Gets and converts the data of the last commit on the Stream
    /// </summary>
    /// <returns></returns>
    public void Receive()
    {
      Debug.Log("Receive Started");
      if (Client == null || string.IsNullOrEmpty(streamShell.streamId))
        Debug.LogException(new Exception("Receiver has not been initialized. Please call Init()."));

      Task.Run(async () =>
      {
        try
        {
          Debug.Log($"Calling to branch {streamShell.branch}");
          // if we have a valid commit to find 

          Commit commit;
          if (!streamShell.commit.Valid())
          {
            var mainBranch = await Client.BranchGet(streamShell.streamId, streamShell.branch);

            if (!mainBranch.commits.items.Any())
              Debug.LogException(new Exception("This branch has no commits"));

            commit = mainBranch.commits.items[0];
          }
          else
          {
            Debug.Log($"Getting Commit {streamShell.commit}");
            commit = await Client.CommitGet(streamShell.streamId, streamShell.commit);
          }

          GetAndConvertObject(commit.referencedObject, commit.id);
        }
        catch (Exception e)
        {
          throw new SpeckleException(e.Message, e, true, SentryLevel.Error);
        }
      });
    }

    #region private methods

    private async void GetAndConvertObject(string objectId, string commitId)
    {
      Debug.Log($"obj id={objectId}");
      try
      {
        //TODO: Replace with new tracker stuff
        // Tracker.TrackPageview(Tracker.RECEIVE);

        var transport = new ServerTransport(Client.Account, streamShell.streamId);
        var @base = await Operations.Receive(
          objectId,
          transport,
          onErrorAction: OnErrorAction,
          onProgressAction: OnProgressAction,
          onTotalChildrenCountKnown: OnTotalChildrenCountKnown,
          disposeTransports: true
        );

        Dispatcher.Instance().Enqueue(() =>
        {
          Debug.Log("Sending to Dispatcher");
          var rc = GetComponent<RecursiveConverter>();
          var go = rc.ConvertRecursivelyToNative(@base, commitId, objectConverter);
          //remove previously received object
          if (DeleteOld && ReceivedData != null)
            Destroy(ReceivedData);
          
          ReceivedData = go;
          OnDataReceivedAction?.Invoke(go);
        });
      }
      catch (Exception e)
      {
        throw new SpeckleException(e.Message, e, true, SentryLevel.Error);
      }

      try
      {
        await Client.CommitReceived(new CommitReceivedInput
        {
          streamId = streamShell.streamId,
          commitId = commitId,
          message = "received commit from " + HostApplications.Unity.Name,
          sourceApplication = HostApplications.Unity.Name
        });
      }
      catch
      {
        // Do nothing!
      }

    }

    private void OnDestroy()
    {
      Client?.CommitCreatedSubscription?.Dispose();
    }
    #endregion

  }
}