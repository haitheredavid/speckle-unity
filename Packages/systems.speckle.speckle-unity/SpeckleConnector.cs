using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  [AddComponentMenu("Speckle/Speckle Connector")]
  public class SpeckleConnector : MonoBehaviour
  {
    [SerializeField] private ConverterUnity converter;
    [SerializeField] private List<Sender> senders = new List<Sender>();
    [SerializeField] private List<Receiver> receivers = new List<Receiver>();

    private void Start()
    {
      Receive().Forget();

    }

    private void OnEnable()
    {
      senders ??= new List<Sender>();
      receivers ??= new List<Receiver>();

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
      // await sender.Init("https://speckle.xyz/streams/4f5b4785b0/");
      // await sender.Send(new List<GameObject>() { o });
    }
  }

}