using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sentry;
using Speckle.Core.Api;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Speckle.ConnectorUnity
{

  /// <summary>
  ///   A Speckle Sender, it's a wrapper around a basic Speckle Client
  ///   that handles conversions for you
  /// </summary>
  [AddComponentMenu("Speckle/Sender")]
  public class Sender : SpeckleClient
  {

    public UnityAction<string> onDataSent;

    public async UniTask<string> Send(List<GameObject> objs = null, string message = null, CancellationTokenSource cancellationToken = null)
    {

      var objectId = "";

      if (!IsReady())
        return objectId;

      // if no objects were passed in we'll try converting from the assigned root object 
      if (objs == null || !objs.Any())
        objs = new List<GameObject>() { root };

      var data = objs.Count > 1 ? ConvertRecursively(objs) : ConvertRecursively(objs[0]);

      try
      {
        ConnectorConsole.Log("Sending data");

        objectId = await Helpers.Send(
          stream.Id,
          data,
          message.Valid() ? message : $"Objects from Unity {data.totalChildrenCount}",
          HostApp,
          account: client.Account,
          onProgressAction: onProgressReport,
          onErrorAction: onErrorReport
        );

        Debug.Log($"data sent! {objectId}");

        onDataSent?.Invoke(objectId);
        UniTask.Yield();
      }

      catch (Exception e)
      {
        Debug.LogException(new SpeckleException(e.Message, e, true, SentryLevel.Error));
      }

      return objectId;

    }

    #region private methods
    private Base ConvertRecursively(IEnumerable<GameObject> objs)
    {
      return new Base()
      {
        ["objects"] = objs.Select(ConvertRecursively).Where(x => x != null).ToList()
      };
    }

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

      if (go != null && go.transform.childCount > 0)
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