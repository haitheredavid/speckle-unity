using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Objects.Converter.Unity;
using Sentry;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using UnityEngine;
using UnityEngine.Events;

namespace Speckle.ConnectorUnity
{
  [AddComponentMenu("Speckle/Speckle Connector")]
  public class SpeckleConnector : MonoBehaviour
  {
    [SerializeField] private StreamShell streamShell;
    [SerializeField] private ConverterUnity converter;
    [SerializeField] private List<Sender> senders = new List<Sender>();
    [SerializeField] private List<Receiver> receivers = new List<Receiver>();

    public Action<string> onDataSent;
    public Action<ConcurrentDictionary<string, int>> onProgressUpdate;
    public Action<string, Exception> onErrorUpdate;

    public UnityEvent<GameObject> onCommitReceived;

    public static bool GenerateMaterials = false;

    private void OnEnable()
    {
      senders ??= new List<Sender>();
      receivers ??= new List<Receiver>();

      onDataSent = commit => Debug.Log($"Commit created {commit}");
      onErrorUpdate = (input, exception) => Debug.LogException(exception);
      onProgressUpdate = res =>
      {
        foreach (var item in res)
          Debug.Log(item.Key + item.Value);
      };
    }

    private void Start()
    {
      onCommitReceived.AddListener(arg0 =>
      {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Send(new List<GameObject> { obj }, "test from unity");
      });
      Receive();

    }

    private void OnDisable()
    {
      // TODO: Clean up tasks
    }

    /// <summary>
    /// Creates a new sender and passed off a list of game objects to be converted and sent to a speckle stream
    /// </summary>
    /// <param name="message"></param>
    /// <param name="shell">ID of the stream to send to</param>
    /// <param name="gameObjects">List of gameObjects to convert and send</param>
    /// <param name="account">Account to use. If not provided the default account will be used</param>
    public void Send(List<GameObject> gameObjects, string message, StreamShell shell = null, Account account = null)
    {
      var sender = new GameObject(nameof(Sender)).AddComponent<Sender>();
      // setup new sender client
      sender.Init(account.thisOrDefault(), shell ?? streamShell, converter, onDataSent, onProgressUpdate, onErrorUpdate);
      // fire and forget this task
      sender.Send(gameObjects, message).Forget();
      // keep track of senders that are created from this manager
      senders.Add(sender);
    }

    public void Receive(StreamShell shell = null, Account account = null)
    {
      var r = new GameObject(nameof(Receiver)).AddComponent<Receiver>();
      // setup new receiver client 
      r.Init(account.thisOrDefault(), shell ?? streamShell, converter, o => onCommitReceived?.Invoke(o), onProgressUpdate, onErrorUpdate);
      // fire and forget
      r.Receive().Forget();
    }

    public static async UniTask<Base> Receive(
      string objectId, ITransport transport, CancellationToken token, Action<string, Exception> onErrorAction, Action<ConcurrentDictionary<string, int>> onProgressAction,
      Action<int> onTotalChildCountKnown, bool dispose
    )
    {
      Base @base = null;
      try
      {
        Debug.Log($"obj id={objectId}");

        @base = await Operations.Receive(objectId, token, transport,
                                         onProgressAction: onProgressAction,
                                         onTotalChildrenCountKnown: onTotalChildCountKnown,
                                         onErrorAction: onErrorAction,
                                         disposeTransports: dispose);

      }
      catch (Exception e)
      {
        Debug.LogException(new SpeckleException(e.Message, e, true, SentryLevel.Error));
      }
      return @base;

    }

    public static async UniTaskVoid CommitReceived(Client client, string streamId, string commitId, string message = null)
    {
      try
      {
        await client.CommitReceived(new CommitReceivedInput
        {
          streamId = streamId,
          commitId = commitId,
          message = message.Valid() ? message : "received commit from " + HostApplications.Unity.Name,
          sourceApplication = HostApplications.Unity.Name
        });
      }
      catch (Exception e)
      {
        Debug.LogException(new SpeckleException(e.Message, e, true, SentryLevel.Error));
      }
    }

    public static async UniTask<Commit> GetCommit(Client client, StreamShell shell)
    {
      Commit commit = null;

      if (client == null || string.IsNullOrEmpty(shell.streamId))
      {
        Debug.LogException(new Exception("Client cannot pull from stream"));
        return null;
      }

      try
      {
        if (shell.commit.Valid())
        {
          Debug.Log($"Getting commit {shell.commit} from {shell.streamId}");
          commit = await client.CommitGet(shell.streamId, shell.commit);
        }
        else
        {
          Debug.Log($"Getting most recent commit from {shell.streamId} on {shell.branch}");

          var mainBranch = await client.BranchGet(shell.streamId, shell.branch);

          if (!mainBranch.commits.items.Any())
            Debug.LogException(new Exception("This branch has no commits"));

          commit = mainBranch.commits.items[0];
        }

        return commit;
      }
      catch (Exception e)
      {
        Debug.LogException(new SpeckleException(e.Message, e, true, SentryLevel.Error));
      }

      return commit;
    }

    public static async UniTask<string> CommitCreate(Client client, StreamShell shell, string objectId, string message, CancellationToken token)
    {
      string res = "";
      try
      {
        res = await client.CommitCreate(token, new CommitCreateInput
        {
          streamId = shell.streamId,
          branchName = shell.branch,
          objectId = objectId,
          message = message,
          sourceApplication = HostApplications.Unity.Name
        });
      }

      catch (Exception e)
      {
        Debug.LogException(new SpeckleException(e.Message, e, true, SentryLevel.Error));
      }

      return res;
    }

    public static async UniTask<string> Send(
      Base @base,
      CancellationToken token,
      List<ITransport> transports,
      bool useDefaultCache,
      Action<ConcurrentDictionary<string, int>> onProgress,
      Action<string, Exception> onError
    )
    {
      var objectId = "";

      try
      {
        objectId = await Operations.Send(@base, token, transports, useDefaultCache, onProgress, onError);
        Analytics.TrackEvent(AccountManager.GetDefaultAccount(), Analytics.Events.Send);
      }
      catch (Exception e)
      {
        Debug.LogException(new SpeckleException(e.Message, e, true, SentryLevel.Error));

      }

      // var objectId = await Helpers.Send()
      return objectId;
    }

    public static async UniTask<string> Send(
      string streamId,
      Base @base,
      string message,
      Account account,
      Action<ConcurrentDictionary<string, int>> onProgress,
      Action<string, Exception> onError
    )
    {
      var objectId = "";
      try
      {
        objectId = await Helpers.Send(streamId, @base, message, HostApplications.Unity.Name, 0, account, true, onProgress, onError);
      }
      catch (Exception e)
      {
        Debug.LogException(new SpeckleException(e.Message, e, true, SentryLevel.Error));
      }

      return objectId;
    }
  }

  public static class ConnectorUtils
  {

    public static Account thisOrDefault(this Account account) => account ?? AccountManager.GetDefaultAccount();

  }
}