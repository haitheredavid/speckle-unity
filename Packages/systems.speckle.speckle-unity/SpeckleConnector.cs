using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Speckle.Core.Credentials;
using UnityEngine;
using UnityEngine.Events;

namespace Speckle.ConnectorUnity
{
  [AddComponentMenu("Speckle/Speckle Connector")]
  public class SpeckleConnector : MonoBehaviour
  {
    [SerializeField] private ConverterUnity converter;
    [SerializeField] private List<Sender> senders = new List<Sender>();
    [SerializeField] private List<Receiver> receivers = new List<Receiver>();

    public UnityEvent<GameObject> onCommitReceived;

    public Action<string> onDataSent;
    public Action<string, Exception> onErrorUpdate;
    public Action<ConcurrentDictionary<string, int>> onProgressUpdate;

    private void Start()
    {
      // onCommitReceived.AddListener(arg0 =>
      // {
      //   var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
      //   Send(new List<GameObject> { obj }, "test from unity");
      // });
      Receive().Forget();

    }

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

    public async UniTaskVoid Receive()
    {
      if (receivers != null)
        foreach (var r in receivers)
        {
          await r.Init("https://speckle.xyz/streams/4f5b4785b0/commits/18c4e05550");
          r.OnDataReceivedAction += obj => Send(obj).Forget();
          r.Receive().Forget();
        }


    }

    private async UniTaskVoid Send(GameObject o)
    {
      var sender = new GameObject().AddComponent<Sender>();
      await sender.Init("https://speckle.xyz/streams/4f5b4785b0/");
      await sender.Send(new List<GameObject>() { o });
    }
  }

  public static class ConnectorUtils
  {

    public static Account thisOrDefault(this Account account) => account ?? AccountManager.GetDefaultAccount();
  }
}