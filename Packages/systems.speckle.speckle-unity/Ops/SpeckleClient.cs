using System;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using Speckle.Core.Api;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  public abstract class SpeckleClient : MonoBehaviour
  {
    [SerializeField] private SpeckleStream rootStream;
    [SerializeField] private ConverterUnity converter;

    private Client client;
    private Action<string, Exception> onErrorReported;

    private Action<ConcurrentDictionary<string, int>> onProgressReport;

    private void OnDisable()
    {
      CleanUp();
    }

    private void OnDestroy()
    {
      CleanUp();
    }

    public async UniTask<bool> Init(
      string streamUrl, ConverterUnity converterUnity = null,
      Action<ConcurrentDictionary<string, int>> onProgressAction = null,
      Action<string, Exception> onErrorAction = null
    )
    {
      if (rootStream == null)
        rootStream = ScriptableObject.CreateInstance<SpeckleStream>();

      rootStream.Init(streamUrl);

      return await Init(rootStream, converterUnity, onProgressAction, onErrorAction);
    }

    public async UniTask<bool> Init(
      SpeckleStream stream = null,
      ConverterUnity converterUnity = null,
      Action<ConcurrentDictionary<string, int>> onProgressAction = null,
      Action<string, Exception> onErrorAction = null
    )
    {
      if (stream != null)
        rootStream = stream;

      if (converterUnity != null)
        converter = converterUnity;

      if (rootStream == null || !rootStream.IsValid())
      {
        ConnectorConsole.Log("Speckle stream object is not setup correctly");
        return false;
      }

      if (converter == null)
      {
        ConnectorConsole.Log($"No converter associated with {name}, stopping conversion");
        return false;
      }

      client = new Client(await rootStream.GetAccount());
      SetSubscriptions();

      return client != null;
    }

    protected virtual void SetSubscriptions()
    {
      if (client == null)
      {
        ConnectorConsole.Log($"No active client on {name} to read from");
        return;
      }
    }

    protected virtual void CleanUp()
    {
      client?.Dispose();
    }
  }
}