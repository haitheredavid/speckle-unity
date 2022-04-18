using System;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using Speckle.Core.Api;
using Speckle.Core.Kits;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  public abstract class SpeckleClient : MonoBehaviour
  {

    protected const string HostApp = HostApplications.Unity.Name;
    [SerializeField] protected GameObject root;
    [SerializeField] protected SpeckleStream stream;
    [SerializeField] protected ConverterUnity converter;

    [Space]
    [SerializeField] private bool expired;

    protected Client client;

    protected bool isCanceled;

    protected Action<string, Exception> onErrorReport;
    protected Action<ConcurrentDictionary<string, int>> onProgressReport;

    private void OnEnable()
    {
      if (converter == null)
        converter = ScriptableObject.CreateInstance<ConverterUnity>();
    }

    private void OnDisable()
    {
      CleanUp();
    }

    private void OnDestroy()
    {
      CleanUp();
    }

    /// <param name="streamUrl">A speckle stream url that will be parsed by the stream wrapper</param>
    /// <param name="converterUnity">Converter to use for sending objects</param>
    /// <param name="onProgressAction">Action to run when there is download/conversion progress</param>
    /// <param name="onErrorAction">Action to run on error</param>
    public async UniTask<bool> Init(
      string streamUrl, ConverterUnity converterUnity = null,
      Action<ConcurrentDictionary<string, int>> onProgressAction = null,
      Action<string, Exception> onErrorAction = null
    )
    {
      if (stream == null)
        stream = ScriptableObject.CreateInstance<SpeckleStream>();

      stream.Init(streamUrl);

      return await Init(stream, converterUnity, onProgressAction, onErrorAction);
    }

    /// <param name="rootStream">root stream object to use, will default to editor field</param>
    /// <param name="converterUnity">Converter to use for sending objects, will default to editor field</param>
    /// <param name="onProgressAction">Action to run when there is download/conversion progress</param>
    /// <param name="onErrorAction">Action to run on error</param>
    public async UniTask<bool> Init(
      SpeckleStream rootStream = null,
      ConverterUnity converterUnity = null,
      Action<ConcurrentDictionary<string, int>> onProgressAction = null,
      Action<string, Exception> onErrorAction = null
    )
    {
      if (rootStream != null)
        stream = rootStream;

      if (converterUnity != null)
        converter = converterUnity;

      if (this.stream == null || !stream.IsValid())
      {
        ConnectorConsole.Log("Speckle stream object is not setup correctly");
        return false;
      }

      if (converter == null)
      {
        ConnectorConsole.Log($"No converter associated with {name}, stopping conversion");
        return false;
      }

      ConnectorConsole.Log("Getting account");

      var account = await this.stream.GetAccount();
      client = new Client(account);
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

    protected bool IsReady()
    {
      var res = true;

      if (stream == null || !stream.IsValid())
      {
        ConnectorConsole.Log($"No active stream ready for {name} to use");
        res = false;
      }

      if (client == null)
      {
        ConnectorConsole.Log($"No active client for {name} to use");
        res = false;
      }

      return res;
    }

    protected virtual void CleanUp()
    {
      client?.Dispose();
    }
  }
}