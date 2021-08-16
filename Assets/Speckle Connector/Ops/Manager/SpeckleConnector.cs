using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;
using UnityEngine.Events;
using ViewTo.Objects.Converter.Unity;
using ViewTo.Objects.Speckle;

namespace ConnectorUnity
{

  [ExecuteAlways]
  [RequireComponent(typeof(SpeckleConverter))]
  public class SpeckleConnector : MonoBehaviour
  {
    [SerializeField] private SpeckleConnectorInput input;
    [SerializeField] private List<Receiver> receivers;
    [SerializeField] private SpeckleConnectorCache cache;

    [SerializeField] private UnityEvent receiveEvent;

    private Client client;

    public static SpeckleConnector Instance { get; set; }
    public static SpeckleConverter Converter { get; set; }

    public bool IsPrimed
    {
      get => client != null && account != null;
    }

    public bool IsReady
    {
      get => IsPrimed && stream != null && branch != null && commit != null;
    }

    #region cache data
    public List<Account> accounts
    {
      get => cache.Accounts;
      private set
      {
        cache.Accounts = value;
        input.Set(value);
      }
    }

    public List<Stream> streams
    {
      get => cache.Streams;
      private set
      {
        cache.Streams = value;
        input.Set(value);
      }
    }

    public List<Branch> branches
    {
      get => cache.Branches;
      private set
      {
        cache.Branches = value;
        input.Set(value);
      }
    }

    public List<Commit> commits
    {
      get => cache.Commits;
      private set
      {
        cache.Commits = value;
        input.Set(value);
      }
    }

    public List<ISpeckleKit> kits
    {
      get => cache.Kits;
      private set
      {
        cache.Kits = value;
        input.Set(value);
      }
    }

    public Account account
    {
      get => cache.Accounts.CheckList(input.account);
    }

    public Stream stream
    {
      get => cache.Streams.CheckList(input.stream);
    }

    public Branch branch
    {
      get => cache.Branches.CheckList(input.branch);
    }

    public Commit commit
    {
      get => cache.Commits.CheckList(input.commit);
    }

    public ISpeckleKit kit
    {
      get => cache.Kits.CheckList(input.kit);
    }
    #endregion

    #region unity logic
    private void OnEnable()
    {
      Instance = this;
      Converter = gameObject.GetComponent<SpeckleConverter>();
      input ??= new SpeckleConnectorInput();
    }

    private async void OnValidate()
    {
      if (!IsPrimed)
      {
        await LoadManager();
        return;
      }

      var states = cache.UpdateAndCheck(input);

      if (!states.Any())
      {
        Debug.Log("Cache is up to date");
        return;
      }


      // some stupid overkill for sorting a list of probably one object... just thinking of protecting from data editor changes...
      var ordered = states.ToDictionary(s => s, s => (int)s).OrderBy(key => key.Value);
      switch (ordered.FirstOrDefault().Key)
      {
        case SpeckleConnectorState.AccountChanged:
          await LoadAccount();
          break;
        case SpeckleConnectorState.StreamChanged:
          await LoadStream();
          break;
        case SpeckleConnectorState.BranchChanged:
          LoadBranch();
          break;
        case SpeckleConnectorState.CommitChanged:
          LoadCommit();
          break;
        case SpeckleConnectorState.KitChanged:
          LoadKit();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
    #endregion

    #region gui
    [Button("Receive")]
    private void CreateReceiver()
    {
      Debug.Log("Recieve Called");

      if (IsReady)
      {

        receivers ??= new List<Receiver>();
        var r = new GameObject().AddComponent<Receiver>();

        // TODO: look into kit references for unity
        ISpeckleConverter converter = default;
        if (kit is ViewToKit)
          converter = new ViewObjUnityConverter();

        r.Init(new StreamShell
                 {streamId = stream.id, branch = branch.name, streamName = stream.name, commitId = commit.id}, account, converter);

        r.Receive();
        receivers.Add(r);
      }
    }

    [Button("Reset")]
    private async void Reset()
    {
      client = null;
      accounts = null;
      streams = null;
      branches = null;
      commits = null;
      kits = null;


      await LoadManager();
    }

    [Button("Clear")]
    private void ClearReceivers()
    {
      Debug.Log("Clear Called");

      if (receivers != null && receivers.Any())
        for (int i = receivers.Count - 1; i >= 0; i--)
          ConnectorUtilities.SafeDestroy(receivers[i].gameObject);

      receivers = new List<Receiver>();

    }
    #endregion

    #region setup
    private async Task LoadManager()
    {
      RefreshKits();
      accounts = AccountManager.GetAccounts().ToList();

      await LoadAccount();
    }

    private async Task LoadAccount()
    {
      try
      {
        if (account != null)
        {
          client = new Client(account);
          streams = await client.StreamsGet();
        }
      }
      catch (Exception e)
      {
        Debug.Log("Account is invalid, or has no streams");
        streams = new List<Stream>();
      }
      await LoadStream();
    }

    private async Task LoadStream()
    {
      if (stream != null)
      {
        branches = await client.StreamGetBranches(stream.id);
      }
      else
      {
        Debug.LogWarning("Error with Loading Stream");
        branches = new List<Branch>();
      }
      LoadBranch();
    }

    private void LoadBranch()
    {
      commits = branch != null ? branch.commits.items : new List<Commit>();
      LoadCommit();
    }

    private void LoadCommit()
    {
      if (commit != null)
      {
        Debug.Log("Manager Primed with commit!\n"
                  + $"id:{commit.id}\n"
                  + $"author:{commit.authorName}\n"
                  + $"msg:{commit.message}");
      }
    }

    private void RefreshKits()
    {
      Debug.Log("Loading Kits");
      kits = KitManager.Kits.ToList();

    }

    private void LoadKit()
    {
      Debug.Log("Selecting Kit");

    }
    #endregion

  }
}