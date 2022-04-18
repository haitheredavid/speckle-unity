using System;
using Cysharp.Threading.Tasks;
using Speckle.Core.Api;
using Speckle.Core.Api.SubscriptionModels;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using UnityEngine;
using UnityEngine.Events;

namespace Speckle.ConnectorUnity
{
  /// <summary>
  ///   A Speckle Receiver, it's a wrapper around a basic Speckle Client
  ///   that handles conversions and subscriptions for you
  /// </summary>
  public class Receiver : SpeckleClient
  {
    public bool autoReceive = false;
    public bool deleteOld = true;

    public UnityAction<GameObject> OnDataReceivedAction;
    public UnityAction<int> OnTotalChildrenCountKnown;

    #region private methods
    private void OnDestroy()
    {
      client?.CommitCreatedSubscription?.Dispose();
    }
    #endregion

    protected override void SetSubscriptions()
    {
      if (client != null && autoReceive)
      {
        client.SubscribeCommitCreated(stream.StreamId);
        client.OnCommitCreated += (sender, commit) => OnCommitCreated?.Invoke(commit);
        client.SubscribeCommitUpdated(stream.StreamId);
        client.OnCommitUpdated += (sender, commit) => OnCommitUpdated?.Invoke(commit);
      }
    }

    /// <summary>
    ///   Gets and converts the data of the last commit on the Stream
    /// </summary>
    /// <returns></returns>
    public async UniTaskVoid Receive()
    {
      ConnectorConsole.Log("Receive Started");
      try
      {
        Base @base = null;

        var commit = await client.CommitGet(stream.StreamId, stream.CommitId);

        var transport = new ServerTransport(client.Account, stream.StreamId);

        ConnectorConsole.Log($"obj id={commit.referencedObject}");

        @base = await Operations.Receive(
          commit.referencedObject,
          this.GetCancellationTokenOnDestroy(),
          transport,
          onProgressAction: onProgressReport,
          onTotalChildrenCountKnown: i => OnTotalChildrenCountKnown?.Invoke(i),
          onErrorAction: onErrorReport);

        if (@base == null)
        {
          ConnectorConsole.Warn("The data pulled from stream was not recieved correctly");
          return;
        }

        ConnectorConsole.Log($"Data with {@base.totalChildrenCount}");

        await client.CommitReceived(new CommitReceivedInput
        {
          streamId = stream.StreamId,
          commitId = commit.id,
          message = $"received commit from {HostApp} ",
          sourceApplication = HostApp
        });


        // TODO: handle the process for update objects and not just force deleting
        //remove previously received object
        if (deleteOld && root != null)
          ConverterUtils.SafeDestroy(root);

        ConnectorConsole.Log("Converting Started");
        //TODO: check if this still converts correctly
        root = converter.ConvertRecursively(@base);

        OnDataReceivedAction?.Invoke(root);
      }
      catch (Exception e)
      {
        ConnectorConsole.Exception(new SpeckleException(e.Message));
        return;
      }
    }

    #region Subscriptions
    public UnityAction<CommitInfo> OnCommitCreated;
    public UnityAction<CommitInfo> OnCommitUpdated;
    #endregion

  }
}