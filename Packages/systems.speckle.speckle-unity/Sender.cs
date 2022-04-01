using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    }
    public void OnDestroy()
    {
      Debug.Log("On Destory called");
    }

    /// <summary>
    ///   Converts and sends the data of the last commit on the Stream
    /// </summary>
    /// <param name="shell">ID of the stream to send to</param>
    /// <param name="gameObjects">List of gameObjects to convert and send</param>
    /// <param name="message">Message to send with the commit</param>
    /// <param name="cancellation">Token for calling cancellation. If nothing is passed the game object will act as the source</param>
    /// <exception cref="SpeckleException"></exception>
    public async UniTaskVoid Send(List<GameObject> gameObjects, string message = null, CancellationTokenSource cancellation = null)
    {
      var @base = ConvertRecursivelyToSpeckle(gameObjects);

      try
      {
        Debug.Log("Sending data");

        var commitId = "";
        // stream id can be the url too if we want to specify branch too 
        (isCanceled, commitId) = await SpeckleConnector.Send(
          streamShell.streamId,
          @base, message,
          client.Account,
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
    private Base ConvertRecursivelyToSpeckle(List<GameObject> gos)
    {
      if (gos.Count == 1) return RecurseTreeToNative(gos[0]);

      var @base = new Base();
      @base["objects"] = gos.Select(x => RecurseTreeToNative(x)).Where(x => x != null).ToList();
      return @base;
    }

    private Base RecurseTreeToNative(GameObject go)
    {
      if (converter == null)
        converter = ScriptableObject.CreateInstance<ConverterUnity>();

      if (converter.CanConvertToSpeckle(go))
        try
        {
          return converter.ConvertToSpeckle(go);
        }
        catch (Exception e)
        {
          Debug.LogException(e);
          return null;
        }

      if (go.transform.childCount > 0)
      {
        var @base = new Base();
        var objects = new List<Base>();
        for (var i = 0; i < go.transform.childCount; i++)
        {
          var goo = RecurseTreeToNative(go.transform.GetChild(i).gameObject);
          if (goo != null)
            objects.Add(goo);
        }

        if (objects.Any())
        {
          @base["objects"] = objects;
          return @base;
        }
      }

      return null;
    }
    #endregion

  }
}