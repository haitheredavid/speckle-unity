using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HaiThere.Utilities;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using UnityEngine;

namespace Speckle_Connector
{
  public static class StreamManagerUtilis
  {
    public static string[] Format(this List<Account> items)
    {
      return items.Valid() ? items.Select(x => x.userInfo.email + " | " + x.serverInfo.name).ToArray() : new[] {"no valid accounts"};
    }
  }

  [ExecuteAlways]
  public class SimpleStreamManager : MonoBehaviour
  {

    public bool Primed { get; set; }

    private Client Client;
    private Account SelectedAccount;
    private Stream SelectedStream;
    
    private List<Account> Accounts;
    private List<Stream> Streams;
    private List<Branch> Branches;

    #region editor
    public string[] GetAccounts => Accounts.Format();
    #endregion

    private void Awake()
    {
      Accounts = new List<Account>();
      Streams = new List<Stream>();
      Branches = new List<Branch>();
      LoadAccounts();
    }

    public async Task LoadAccounts()
    {

      Primed = false;
      Accounts = AccountManager.GetAccounts().ToList();
      if (!Accounts.Any())
      {
        Debug.Log("No Accounts found, please login in Manager");
      } else
      {
        Primed = true;
        await SelectAccount(0);
      }
    }

    public async Task SelectAccount(int i)
    {

      // SelectedAccount = Accounts[i];

      // Client = new Client(SelectedAccount);
      // await LoadStreams();
    }

  }
}