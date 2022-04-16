using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sentry;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity
{

  /// <summary>
  ///   A Speckle Sender, it's a wrapper around a basic Speckle Client
  ///   that handles conversions for you
  /// </summary>
  public class Sender : MonoBehaviour
  {
    public bool useDefaultCache = true;
    [SerializeField] private StreamShell streamShell;
    [SerializeField] private ConverterUnity converter;

    private bool isCanceled = false;

    private Client client;
    private Action<string> onDataSent;
    private Action<ConcurrentDictionary<string, int>> onProgressReport;
    private Action<string, Exception> onErrorReported;
    /// <param name="shell">stream to send data to</param>
    /// <param name="converterUnity">Converter to use for sending objects</param>
    /// <param name="account">Account to use. If not provided the default account will be used</param>
    /// <param name="onDataSentAction">Action to run after the data has been sent</param>
    /// <param name="onProgressAction">Action to run when there is download/conversion progress</param>
    /// <param name="onErrorAction">Action to run on error</param>
    public void Init(
      Account account,
      StreamShell shell = null,
      ConverterUnity converterUnity = null,
      Action<string> onDataSentAction = null,
      Action<ConcurrentDictionary<string, int>> onProgressAction = null,
      Action<string, Exception> onErrorAction = null
    )
    {
      // use either the params or the data stored here
      if (shell != null) streamShell = shell;

      if (converterUnity != null) converter = converterUnity;

      client = new Client(account);

      onDataSent = onDataSentAction;
      onProgressReport = onProgressAction;
      onErrorReported = onErrorAction;

      if (converter == null)
        converter = ScriptableObject.CreateInstance<ConverterUnity>();
    }
    public void OnDestroy()
    {
      Debug.Log("On Destory called");
    }

    public async UniTaskVoid Send(List<GameObject> objs, string message = null, CancellationTokenSource cancellationToken = null)
    {
      var data = objs.Count > 1 ? new Base
          {
            ["objects"] = objs.Select(ConvertRecursively).Where(x => x != null).ToList()
          }
          : ConvertRecursively(objs[0])
        ;

      try
      {
        Debug.Log("Sending data");

        var commitId = "";
        // stream id can be the url too if we want to specify branch too 
        (isCanceled, commitId) = await SpeckleConnector.Send(
          streamId: streamShell.streamId,
          data,
          message: message.Valid() ? message : $"Objects from Unity {data.totalChildrenCount}",
          account: client.Account,
          onProgress: onProgressReport,
          onError: onErrorReported).SuppressCancellationThrow();

        // NOTE: Calling from operations is causing an issue with unity but the helper one works just fine
        // var transport = new ServerTransport(client.Account, streamShell.streamId);
        // (isCanceled, objectId) = await SpeckleConnector.Send(
        //   @base,
        //   this.GetCancellationTokenOnDestroy(),
        //   new List<ITransport>() { transport },
        //   true,
        //   onProgressReport,
        //   onErrorReported
        // ).SuppressCancellationThrow();

        // checks if UniTask was cancelled

        if (isCanceled)
        {
          Debug.LogWarning("Data sent to speckle was cancelled");
          return;
        }

        Debug.Log($"data sent! {commitId}");
        onDataSent?.Invoke(commitId);
        UniTask.Yield();
      }
      catch (Exception e)
      {
        Debug.LogException(new SpeckleException(e.Message, e, true, SentryLevel.Error));
      }
    }

    #region private methods
    private Base ConvertRecursively(GameObject go)
    {
      if (converter.CanConvertToSpeckle(go))
        try
        {
          return converter.ConvertToSpeckle(go);
        }
        catch (Exception e)
        {
          Debug.LogException(e);
        }

      return CheckForChildren(go, out var objs) ?
        new Base { ["objects"] = objs } : null;
    }

    private bool CheckForChildren(GameObject go, out List<Base> objs)
    {
      objs = new List<Base>();

      if (go.transform.childCount > 0)
      {
        foreach (Transform child in go.transform)
        {
          var converted = ConvertRecursively(child.gameObject);
          if (converted != null)
            objs.Add(converted);
        }
      }

      return objs.Any();
    }
    #endregion

  }
}