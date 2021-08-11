using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Speckle.ConnectorUnity;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using UnityEngine;

namespace Speckle_Connector
{

  [ExecuteAlways]
  public class SimpleStreamManager : MonoBehaviour
  {

    public enum ManagerState
    {
      Primed,
      AccountSelected,
      AccountLoaded,
      StreamSelected,
      BranchSelected,
      CommitSelected,
      NoAccounts,
      NoStreams,
      NoCommits,
      ClientError
    }

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

    private ManagerCache cache { get; set; } = new ManagerCache();

    private Account account => accounts.Valid() && accounts.Count > cache.account ? accounts[cache.account] : null;
    private Stream stream => streams.Valid() && streams.Count > cache.stream ? streams[cache.stream] : null;
    private Branch branch => branches.Valid() && branches.Count > cache.branch ? branches[cache.branch] : null;
    private Commit commit => commits.Valid() && commits.Count > cache.commit ? commits[cache.commit] : null;

    public List<Account> accounts { get; private set; } = new List<Account>();
    public List<Stream> streams { get; private set; } = new List<Stream>();
    public List<Branch> branches { get; private set; } = new List<Branch>();
    public List<Commit> commits { get; private set; } = new List<Commit>();

    public event Action<ManagerState> StateChangeEvent;

    private async void OnEnable()
    {
      await PrimeManager();
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

      cache.account = LoadFromList(index, accounts);
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
        cache.stream = LoadFromList(index, streams);
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

    public void SelectBranch(int index = 0)
    {
      Debug.Log("Selecting Branch");
      cache.branch = LoadFromList(index, branches);

      commits = branch != null ? branch.commits.items : new List<Commit>();
      StateChangeEvent?.Invoke(ManagerState.BranchSelected);
      SelectCommit();
    }

    public void SelectCommit(int index = 0)
    {
      Debug.Log("Selecting Commit");
      cache.commit = LoadFromList(index, commits);
      StateChangeEvent?.Invoke(commits == null || !commits.Any() ? ManagerState.NoCommits : ManagerState.CommitSelected);
    }

    internal async Task SetInput(ManagerCache input)
    {
      if (cache.account != input.account)
        await SelectAccount(input.account);
      else if (cache.stream != input.stream)
        await SelectStream(input.stream);
      else if (cache.branch != input.branch)
        SelectBranch(input.branch);
      else if (cache.commit != input.commit)
        SelectCommit(input.commit);
      else
        Debug.Log("Cache is up to date");
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
  }
}