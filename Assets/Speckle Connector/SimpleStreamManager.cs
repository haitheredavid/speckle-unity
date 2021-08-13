using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using UnityEngine;

namespace ConnectorUnity
{

  [ExecuteAlways]
  public class SimpleStreamManager : MonoBehaviour
  {

    [HideInInspector]
    [SerializeField] private InputDataShell dataShell = new InputDataShell();

    [SerializeField] private int accountIndex, streamIndex, branchIndex, commitIndex, kitIndex;

    [HideInInspector]
    [SerializeField] private string[] testvalues = new string[]{"test"};
    
    [HideInInspector]
    [SerializeField] private bool inProcess, isPrimed, isValid;

    public bool IsPrimed
    {
      get => account != null && client != null;
    }

    public bool IsValid
    {
      get => IsPrimed && stream != null && branch != null && commit != null;
    }

    public bool InProcess { get; private set; }

    private Client client;

    
    private Account account => accounts.Valid() && accounts.Count > dataShell.account ? accounts[dataShell.account] : null;
    private Stream stream => streams.Valid() && streams.Count > dataShell.stream ? streams[dataShell.stream] : null;
    private Branch branch => branches.Valid() && branches.Count > dataShell.branch ? branches[dataShell.branch] : null;
    private Commit commit => commits.Valid() && commits.Count > dataShell.commit ? commits[dataShell.commit] : null;
    private ISpeckleKit kit => kits.Valid() && kits.Count > dataShell.kit ? kits[dataShell.kit] : null;

    public List<Account> accounts { get; private set; } = new List<Account>();
    public List<Stream> streams { get; private set; } = new List<Stream>();
    public List<Branch> branches { get; private set; } = new List<Branch>();
    public List<Commit> commits { get; private set; } = new List<Commit>();
    public List<ISpeckleKit> kits { get; private set; } = new List<ISpeckleKit>();

    public event Action<ManagerState> StateChangeEvent;

    private void OnValidate()
    {

      // if (!inProcess && IsStale())
      // {
      //   Debug.Log("Cache is stale");
      //   await manager.SetInput(cache);
      // }
    }

    private async void OnEnable()
    {
      RefreshKits();
      await PrimeManager();
    }

    public async Task SetInput(InputDataShell input)
    {
      if (dataShell.account != input.account)
        await SelectAccount(input.account);
      else if (dataShell.stream != input.stream)
        await SelectStream(input.stream);
      else if (dataShell.branch != input.branch)
        SelectBranch(input.branch);
      else if (dataShell.commit != input.commit)
        SelectCommit(input.commit);
      else if (dataShell.kit != input.kit)
        SelectKit(input.kit);
      else
        Debug.Log("Cache is up to date");
    }
    public void RefreshKits()
    {
      Debug.Log("Loading Kits");
      kits = KitManager.Kits.ToList();
    }

    public async Task PrimeManager()
    {
      if (InProcess)
      {
        Debug.Log("Trying to toggle process while already in action");
        return;
      }
      InProcess = true;
      StateChangeEvent?.Invoke(ManagerState.Primed);

      accounts = AccountManager.GetAccounts().ToList();

      if (!accounts.Any())
      {
        StateChangeEvent?.Invoke(ManagerState.NoAccounts);
        return;
      }

      await SelectAccount();
      InProcess = false;
    }

    public async Task SelectAccount(int index = 0)
    {
      Debug.Log("Selecting Account");

      dataShell.account = LoadFromList(index, accounts);
      if (account == null)
      {
        StateChangeEvent?.Invoke(ManagerState.ClientError);
        Debug.LogWarning("Did Not Create Client");
        return;
      }

      StateChangeEvent?.Invoke(ManagerState.AccountSelected);
      Debug.Log("Creating Client");

      try
      {
        client = new Client(account);
        streams = await client.StreamsGet();
      }
      catch (Exception e)
      {
        Debug.Log("Something is weird with this Account\n" + e);
        streams = new List<Stream>();
      }
      finally
      {
        if (streams != null && streams.Any())
        {
          StateChangeEvent?.Invoke(ManagerState.AccountLoaded);
          await SelectStream();
        }
        else
          StateChangeEvent?.Invoke(ManagerState.NoStreams);
      }
    }

    private async Task SelectStream(int index = 0)
    {
      Debug.Log("Selecting Stream");
      try
      {
        dataShell.stream = LoadFromList(index, streams);
        branches = await client.StreamGetBranches(stream.id);
      }
      catch (Exception e)
      {
        Debug.LogWarning("Error with Loading Stream\n" + e);
      }
      finally
      {
        if (stream != null)
        {
          SelectBranch();
          StateChangeEvent?.Invoke(ManagerState.StreamSelected);
        }
      }
    }

    private void SelectBranch(int index = 0)
    {
      Debug.Log("Selecting Branch");
      dataShell.branch = LoadFromList(index, branches);

      commits = branch != null ? branch.commits.items : new List<Commit>();
      StateChangeEvent?.Invoke(ManagerState.BranchSelected);
      SelectCommit();
    }

    private void SelectCommit(int index = 0)
    {
      Debug.Log("Selecting Commit");
      dataShell.commit = LoadFromList(index, commits);
      StateChangeEvent?.Invoke(commits == null || !commits.Any() ? ManagerState.NoCommits : ManagerState.CommitSelected);
    }

    private void SelectKit(int index = 0)
    {
      Debug.Log("Selecting Kit");
      dataShell.kit = LoadFromList(index, kits);
      StateChangeEvent?.Invoke(ManagerState.KitSelected);
    }

    private static int LoadFromList(int index, ICollection list)
    {
      if (list == null)
      {
        Debug.LogWarning("List is null");
        index = 0;
      }
      else if (index >= list.Count)
      {
        Debug.LogWarning($"{list.GetType().ToString().Split('.').Last()} is out of range for that index, defaulting to first index");
        index = 0;
      }

      return index;
    }

    public void Load()
    {
      Debug.Log("Loading to reciever!");
    }
  }
}