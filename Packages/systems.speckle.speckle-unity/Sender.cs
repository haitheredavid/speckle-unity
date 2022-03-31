using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    [SerializeField] private ConverterUnity converter;

    private CancellationTokenSource cancellation;

    private void OnEnable()
    {
      cancellation?.Dispose();
    }

    private void OnDisable()
    {
      cancellation?.Cancel();
      cancellation?.Dispose();
    }
    
    // TODO: should we be sending a list of game objects?
    /// <summary>
    ///   Converts and sends the data of the last commit on the Stream
    /// </summary>
    /// <param name="shell">ID of the stream to send to</param>
    /// <param name="gameObjects">List of gameObjects to convert and send</param>
    /// <param name="message"></param>
    /// <param name="converterUnity">Converter to use for sending objects</param>
    /// <param name="account">Account to use. If not provided the default account will be used</param>
    /// <param name="onDataSentAction">Action to run after the data has been sent</param>
    /// <param name="onProgressAction">Action to run when there is download/conversion progress</param>
    /// <param name="onErrorAction">Action to run on error</param>
    /// <exception cref="SpeckleException"></exception>
    public async void Send(
      StreamShell shell, List<GameObject> gameObjects, string message = null, ConverterUnity converterUnity = null, Account account = null,
      Action<string> onDataSentAction = null,
      Action<ConcurrentDictionary<string, int>> onProgressAction = null,
      Action<string, Exception> onErrorAction = null
    )
    {
      if (converterUnity != null)
        converter = converterUnity;

      Debug.Log("converting data");
      cancellation = new CancellationTokenSource();

      var data = ConvertRecursivelyToSpeckle(gameObjects);

      try
      {
        var transport = new ServerTransport(AccountManager.GetDefaultAccount(), shell.streamId);

        Debug.Log("Sending data");

        var objectId = await Operations.Send(
          data, cancellation.Token, new List<ITransport> { transport },
          true,
          onProgressAction,
          onErrorAction);

        var client = new Client(account ?? AccountManager.GetDefaultAccount());

        Debug.Log("creating Commit");

        var res = await client.CommitCreate(cancellation.Token, new CommitCreateInput
        {
          streamId = shell.streamId,
          branchName = shell.branch,
          objectId = objectId,
          message = message,
          sourceApplication = HostApplications.Unity.Name
        });

        onDataSentAction?.Invoke(res);
      }
      catch (Exception e)
      {
        throw new SpeckleException(e.Message, e, true, SentryLevel.Error);
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