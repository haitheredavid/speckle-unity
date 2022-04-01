using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Objects.Converter.Unity;
using Sentry;
using Speckle.Core.Api;
using Speckle.Core.Api.SubscriptionModels;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Models;
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
    [SerializeField] private GameObject receivedData;

    public bool autoReceive = false;
    public bool deleteOld = true;

    [SerializeField] private ConverterUnity converter;

    private bool isCanceled = false;

    private Action<GameObject> OnDataReceivedAction;
    private Action<string, Exception> OnErrorAction;
    private Action<ConcurrentDictionary<string, int>> OnProgressAction;
    private Action<int> OnTotalChildrenCountKnown;

    private Client client { get; set; }

    private void OnEnable()
    {
      if (converter == null)
        converter = ScriptableObject.CreateInstance<ConverterUnity>();
    }

    /// <summary>
    ///   Initializes the Receiver manually
    /// </summary>
    /// <param name="account">Account to use, if null the default account will be used</param>
    /// <param name="shell">Id of the stream to receive</param>
    /// <param name="converterUnity">Object converter to use</param>
    /// <param name="onDataReceivedAction">Action to run after new data has been received and converted</param>
    /// <param name="onProgressAction">Action to run when there is download/conversion progress</param>
    /// <param name="onErrorAction">Action to run on error</param>
    /// <param name="onTotalChildrenCountKnown">Action to run when the TotalChildrenCount is known</param>
    public void Init(
      Account account,
      StreamShell shell = null,
      ConverterUnity converterUnity = null,
      Action<GameObject> onDataReceivedAction = null,
      Action<ConcurrentDictionary<string, int>> onProgressAction = null,
      Action<string, Exception> onErrorAction = null,
      Action<int> onTotalChildrenCountKnown = null
    )
    {
      // use either the params or the data stored here
      if (shell != null) streamShell = shell;

      if (converterUnity != null) converter = converterUnity;

      client = new Client(account);

      OnDataReceivedAction = onDataReceivedAction;
      OnErrorAction = onErrorAction;
      OnProgressAction = onProgressAction;
      OnTotalChildrenCountKnown = onTotalChildrenCountKnown;


      if (autoReceive)
      {
        client.SubscribeCommitCreated(streamShell.streamId);
        // Client.OnCommitCreated += Client_OnCommitCreated;
      }
    }

    /// <summary>
    ///   Gets and converts the data of the last commit on the Stream
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid Receive()
    {
      Debug.Log("Receive Started");

      Commit commit = null;
      (isCanceled, commit) = await SpeckleConnector.GetCommit(client, streamShell).SuppressCancellationThrow();


      if (isCanceled || commit == null)
      {
        Debug.LogWarning("The commit being asked for was not recieved correctly");
        return;
      }

      var transport = new ServerTransport(client.Account, streamShell.streamId);
      Base @base = null;

      (isCanceled, @base) = await SpeckleConnector.Receive(
        commit.referencedObject,
        transport,
        this.GetCancellationTokenOnDestroy(),
        onProgressAction: OnProgressAction,
        onErrorAction: OnErrorAction,
        onTotalChildCountKnown: OnTotalChildrenCountKnown,
        dispose: true).SuppressCancellationThrow();

      if (@base == null || isCanceled)
      {
        Debug.LogWarning("The data pulled from stream was not recieved correctly");
        return;
      }

      // This part doesn't impact the conversion process so let's forget about it
      SpeckleConnector.CommitReceived(client, streamShell.streamId, commit.id).Forget();


      var rc = GetComponent<RecursiveConverter>();
      var go = rc.ConvertRecursivelyToNative(@base, commit.id, converter);

      if (go == null)
      {
        Debug.LogWarning("Speckle Object was not converted properly");
        return;
      }

      //remove previously received object
      if (deleteOld && receivedData != null)
        Destroy(receivedData);

      receivedData = go;
      OnDataReceivedAction?.Invoke(go);
    }

    #region private methods
    private void OnDestroy()
    {
      client?.CommitCreatedSubscription?.Dispose();
    }
    #endregion

  }
}