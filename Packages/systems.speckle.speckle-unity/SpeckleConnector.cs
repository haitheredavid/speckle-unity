using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Logging;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Speckle.ConnectorUnity
{

  [AddComponentMenu("Speckle/Speckle Connector")]
  [ExecuteAlways]
  public class SpeckleConnector : MonoBehaviour
  {
    [SerializeField] private ConverterUnity converter;
    [SerializeField] private List<SpeckleStream> streams = new List<SpeckleStream>();

    [SerializeField] private List<Sender> senders = new List<Sender>();
    [SerializeField] private List<Receiver> receivers = new List<Receiver>();

    [SerializeField] private List<ConverterUnity> converters = new List<ConverterUnity>();

    [SerializeField] private SpeckleStream stream;
    [SerializeField] private SpeckleStream cachedStream;

    [SerializeField] private int accountIndex;
    [SerializeField] private int streamIndex;

    private Client client;

    public List<Account> Accounts { get; private set; }

    public List<SpeckleStream> Streams
    {
      get => streams;
    }

    public Account activeAccount
    {
      get => Accounts.Valid(accountIndex) ? Accounts[accountIndex] : null;
    }

    public SpeckleStream activeStream
    {
      get => streams.Valid(streamIndex) ? streams[streamIndex] : null;
    }

    private void OnEnable()
    {
      senders ??= new List<Sender>();
      receivers ??= new List<Receiver>();

      Accounts = AccountManager.GetAccounts().ToList();

      SetAccount(accountIndex).Forget();
    }

    public void SetStream(int index)
    {
      streamIndex = Streams.Check(index);
    }

    public async UniTask SetAccount(int index)
    {
      try
      {
        if (Accounts == null)
        {
          ConnectorConsole.Warn("Accounts are not set properly to this connector");
          return;
        }

        streams = new List<SpeckleStream>();

        client = null;
        streamIndex = 0;

        accountIndex = Accounts.Check(index);

        if (activeAccount != null)
        {
          client = new Client(activeAccount);
          var res = await client.StreamsGet();
          streams = new List<SpeckleStream>();

          foreach (var s in res)
          {
            var wrapper = ScriptableObject.CreateInstance<SpeckleStream>();
            wrapper.Init(s.id, activeAccount.userInfo.id, client.ServerUrl, s.name, s.description);
            streams.Add(wrapper);
          }
        }
      }
      catch (SpeckleException e)
      {
        ConnectorConsole.Warn(e.Message);
      }
      finally
      {
        onRepaint?.Invoke();
      }
    }

    #if UNITY_EDITOR
    public static List<T> GetAllInstances<T>() where T : ScriptableObject
    {
      var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
      var items = new List<T>();
      foreach (var g in guids)
      {
        string path = AssetDatabase.GUIDToAssetPath(g);
        items.Add(AssetDatabase.LoadAssetAtPath<T>(path));
      }
      return items;
    }

    #endif

    public void OpenStreamInBrowser(EventBase obj)
    {
      Debug.Log(obj);
    }

    public void CreateSender(EventBase obj)
    {
      if (activeStream == null)
      {
        ConnectorConsole.Log("No Active stream ready to be sent to sender");
      }
    }

    public void CreateReceiver(EventBase obj)
    {
      if (activeStream == null)
      {
        ConnectorConsole.Log("No Active stream ready to be sent to Receiver");
        return;
      }

      UniTask.Create(async () =>
      {
        var mono = new GameObject().AddComponent<Receiver>();

        #if UNITY_EDITOR
        Selection.activeObject = mono;

        #endif

        await mono.Init(activeStream);
      });
    }
  }

}