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
    public string StreamId;
    public string BranchName = "main";
    public int TotalChildrenCount;
    public GameObject ReceivedData;

    private bool AutoReceive;
    private bool DeleteOld;
    private Action<GameObject> OnDataReceivedAction;
    private Action<string, Exception> OnErrorAction;
    private Action<ConcurrentDictionary<string, int>> OnProgressAction;
    private Action<int> OnTotalChildrenCountKnown;
    public Stream Stream;

    [SerializeField] private ConverterUnity objectConverter;

    private Client Client { get; set; }

    /// <summary>
    ///   Initializes the Receiver manually
    /// </summary>
    /// <param name="streamId">Id of the stream to receive</param>
    /// <param name="autoReceive">If true, it will automatically receive updates sent to this stream</param>
    /// <param name="deleteOld">If true, it will delete previously received objects when new one are received</param>
    /// <param name="account">Account to use, if null the default account will be used</param>
    /// <param name="converter">Object converter to use </param>
    /// <param name="onDataReceivedAction">Action to run after new data has been received and converted</param>
    /// <param name="onProgressAction">Action to run when there is download/conversion progress</param>
    /// <param name="onErrorAction">Action to run on error</param>
    /// <param name="onTotalChildrenCountKnown">Action to run when the TotalChildrenCount is known</param>
    public void Init(
      string streamId, bool autoReceive = false, bool deleteOld = true, Account account = null, ConverterUnity converter = null,
      Action<GameObject> onDataReceivedAction = null, Action<ConcurrentDictionary<string, int>> onProgressAction = null,
      Action<string, Exception> onErrorAction = null, Action<int> onTotalChildrenCountKnown = null
    )
    {
      StreamId = streamId;
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
        Client.SubscribeCommitCreated(StreamId);
        Client.OnCommitCreated += Client_OnCommitCreated;
      }
    }

    /// <summary>
    ///   Gets and converts the data of the last commit on the Stream
    /// </summary>
    /// <returns></returns>
    public void Receive()
    {
      Debug.Log("Receive Started");
      if (Client == null || string.IsNullOrEmpty(StreamId))
        Debug.LogException(new Exception("Receiver has not been initialized. Please call Init()."));

      Task.Run(async () =>
      {
        try
        {
          Debug.Log($"Calling to branch {BranchName}");
          var mainBranch = await Client.BranchGet(StreamId, BranchName, 1);
          if (!mainBranch.commits.items.Any())
            Debug.LogException(new Exception("This branch has no commits"));

          var commit = mainBranch.commits.items[0];
          Debug.Log($"commit id={commit}");
          GetAndConvertObject(commit.referencedObject, commit.id);
        }
        catch (Exception e)
        {
          throw new SpeckleException(e.Message, e, true, SentryLevel.Error);
        }
      });
    }

    #region private methods
    /// <summary>
    ///   Fired when a new commit is created on this stream
    ///   It receives and converts the objects and then executes the user defined _onCommitCreated action.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Client_OnCommitCreated(object sender, CommitInfo e)
    {
      if (e.branchName == BranchName)
      {
        Debug.Log("New commit created");
        GetAndConvertObject(e.objectId, e.id);
      }
    }

    private async void GetAndConvertObject(string objectId, string commitId)
    {
      Debug.Log($"obj id={objectId}");
      try
      {
        //TODO: Replace with new tracker stuff
        // Tracker.TrackPageview(Tracker.RECEIVE);

        var transport = new ServerTransport(Client.Account, StreamId);
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
          streamId = StreamId,
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