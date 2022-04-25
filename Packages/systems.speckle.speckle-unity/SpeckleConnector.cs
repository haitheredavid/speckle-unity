using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Logging;
using UnityEngine;

namespace Speckle.ConnectorUnity
{

  [AddComponentMenu("Speckle/Speckle Connector")]
  [ExecuteAlways]
  public class SpeckleConnector : MonoBehaviour
  {
    [SerializeField] private ConverterUnity converter;

    [SerializeField] private List<Sender> senders = new List<Sender>();
    [SerializeField] private List<Receiver> receivers = new List<Receiver>();

    [SerializeField] private SpeckleStream stream;

    [SerializeField] private int accountIndex;
    [SerializeField] private int streamIndex;
    [SerializeField] private int branchIndex;
    [SerializeField] private int commitIndex;
    [SerializeField] private int kitIndex;

    private Client client;

    public List<Account> accounts { get; set; }
    public List<Stream> streams { get; private set; }
    public List<Branch> branches { get; private set; }
    public List<Commit> commits { get; private set; }

    public Account activeAccount
    {
      get => accounts.Valid(accountIndex) ? accounts[accountIndex] : null;
    }

    public Stream activeStream
    {
      get => streams.Valid(streamIndex) ? streams[streamIndex] : null;
    }

    public Branch activeBranch
    {
      get => branches.Valid(branchIndex) ? branches[branchIndex] : null;
    }

    public Commit activeCommit
    {
      get => commits.Valid(commitIndex) ? commits[commitIndex] : null;
    }

    private void OnEnable()
    {
      senders ??= new List<Sender>();
      receivers ??= new List<Receiver>();

      accounts = AccountManager.GetAccounts().ToList();
      LoadAccount().Forget();
    }

    //
    // private void Start()
    // {
    //   Receive().Forget();
    // }

    public async UniTask LoadAccount(int index = -1)
    {
      try
      {
        if (accounts == null)
        {
          ConnectorConsole.Warn("Accounts are not set properly to this connector");
          return;
        }

        client = null;
        streamIndex = 0;

        accountIndex = Check(accounts, index);

        if (activeAccount != null)
        {
          client = new Client(activeAccount);
          streams = await client.StreamsGet();
        }
      }
      catch (SpeckleException e)
      {
        ConnectorConsole.Warn(e.Message);
        streams = new List<Stream>();
      }
      finally
      {
        await LoadStream(streamIndex);
      }
    }

    public async UniTask LoadStream(int index = -1)
    {
      try
      {
        branchIndex = 0;
        commitIndex = 0;

        if (client == null && activeAccount != null)
          client = new Client(activeAccount);

        streamIndex = Check(streams, index);

        if (activeStream != null)
          branches = await client.StreamGetBranches(activeStream.id, 20, 20);

        if (branches != null)
        {
          for (int bIndex = 0; bIndex < branches.Count; bIndex++)
          {
            if (branches[bIndex].name.Equals("main"))
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
        branches = new List<Branch>();
      }
    }

    public void LoadBranch(int i = 0)
    {
      branchIndex = Check(branches, i);
      Debug.Log($"Loading new branch {activeBranch}");

      commits = activeBranch != null ? activeBranch.commits.items : new List<Commit>();
      LoadCommit();
    }

    public void LoadCommit(int i = 0)
    {
      commitIndex = Check(commits, i);

      if (activeCommit != null)
        ConnectorConsole.Log("Active commit loaded! " + activeCommit);
    }

    private static int Check(IList list, int index)
    {
      var res = list.Valid(index) ? index : 0;
      Debug.Log($"checking for index {index} and returning {res}");
      return res;
    }
  }

}