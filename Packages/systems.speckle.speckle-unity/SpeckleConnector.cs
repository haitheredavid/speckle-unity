using System;
using System.Collections;
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
    [SerializeField] private int branchIndex;
    [SerializeField] private int commitIndex;
    [SerializeField] private int converterIndex;

    private Client client;

    public List<Account> Accounts { get; private set; }
    public List<Stream> Streams { get; private set; }
    public List<Branch> Branches { get; private set; }
    public List<Commit> Commits { get; private set; }

    public List<ConverterUnity> Converters
    {
      get => converters.Valid() ? converters : new List<ConverterUnity>();
    }

    public Account activeAccount
    {
      get => Accounts.Valid(accountIndex) ? Accounts[accountIndex] : null;
    }

    public Stream activeStream
    {
      get => Streams.Valid(streamIndex) ? Streams[streamIndex] : null;
    }

    public Branch activeBranch
    {
      get => Branches.Valid(branchIndex) ? Branches[branchIndex] : null;
    }

    public Commit activeCommit
    {
      get => Commits.Valid(commitIndex) ? Commits[commitIndex] : null;
    }

    private void OnEnable()
    {
      senders ??= new List<Sender>();
      receivers ??= new List<Receiver>();

      Accounts = AccountManager.GetAccounts().ToList();

      LoadAccount().Forget();

      // TODO: during the build process this should compile and store these objects. 
      #if UNITY_EDITOR
      converters = GetAllInstances<ConverterUnity>();
      #endif
    }

    public event Action onRepaint;

    public async UniTask<Texture2D> GetPreview()
    {
      if (cachedStream == null)
      {
        ConnectorConsole.Warn("Connector is not ready to load a stream! Try setting the stream parameters first");
        return null;
      }

      return await cachedStream.GetPreview();
    }

    public async UniTask LoadAccount(int index = -1)
    {
      try
      {
        if (Accounts == null)
        {
          ConnectorConsole.Warn("Accounts are not set properly to this connector");
          return;
        }

        client = null;
        streamIndex = 0;

        accountIndex = Check(Accounts, index);

        if (activeAccount != null)
        {
          client = new Client(activeAccount);
          Streams = await client.StreamsGet();
        }
      }
      catch (SpeckleException e)
      {
        ConnectorConsole.Warn(e.Message);
        Streams = new List<Stream>();
      }
      finally
      {
        await LoadStream(streamIndex);
        onRepaint?.Invoke();
      }
    }

    public void SetStream(int index)
    {
      streamIndex = Check(Streams, index);
    }

    public async UniTask LoadStream(int index = -1)
    {
      ConnectorConsole.Log($"Loading new stream at {index}");
      try
      {
        branchIndex = 0;
        Branches = new List<Branch>();

        commitIndex = 0;
        Commits = new List<Commit>();


        if (client == null && activeAccount != null)
          client = new Client(activeAccount);

        streamIndex = Check(Streams, index);

        if (activeStream != null)
        {
          Branches = await client.StreamGetBranches(activeStream.id, 20, 20);
        }

        if (Branches != null)
        {
          for (int bIndex = 0; bIndex < Branches.Count; bIndex++)
          {
            if (Branches[bIndex].name.Equals("main"))
            {
              LoadBranch(bIndex);
              break;
            }
          }
        }
      }
      catch (SpeckleException e)
      {
        ConnectorConsole.Warn(e.Message);
        Branches = new List<Branch>();
      }
    }

    public void LoadBranch(int i = -1)
    {
      branchIndex = Check(Branches, i);

      Commits = activeBranch != null ? activeBranch.commits.items : new List<Commit>();
      LoadCommit();
    }

    public void LoadCommit(int i = -1)
    {
      commitIndex = Check(Commits, i);

      if (activeCommit != null)
        ConnectorConsole.Log("Active commit loaded! " + activeCommit);

      SetCache();
    }

    private void SetCache()
    {
      if (activeAccount == null || activeStream == null)
      {
        ConnectorConsole.Warn("No Account or Stream active, cannot update catch");
        return;
      }

      // build new cached stream
      cachedStream ??= ScriptableObject.CreateInstance<SpeckleStream>();

      if (activeCommit != null)
        cachedStream.Init($"{activeAccount.serverInfo.url}/streams/{activeStream.id}/commits/{activeCommit.id}");
      else if (activeBranch != null)
        cachedStream.Init($"{activeAccount.serverInfo.url}/streams/{activeStream.id}/branches/{activeBranch.name}");
      else
        cachedStream.Init(activeStream.id, activeAccount.userInfo.id, activeAccount.serverInfo.url);
    }

    private static int Check(IList list, int index)
    {
      return list.Valid(index) ? index : 0;
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
      throw new NotImplementedException();
    }
    public void CreateSender(EventBase obj)
    {
      throw new NotImplementedException();
    }
    public void CreateReceiver(EventBase obj)
    {
      throw new NotImplementedException();
    }
  }

}