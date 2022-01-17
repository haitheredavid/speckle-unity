using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  [ExecuteAlways]
  [AddComponentMenu("Speckle/Stream Manager")]
  public class StreamManager : MonoBehaviour
  {

    #if UNITY_EDITOR
    public static bool GenerateMaterials = false;
    #endif

    public int SelectedAccountIndex = -1;
    public int SelectedStreamIndex = -1;
    public int SelectedBranchIndex = -1;
    public int SelectedCommitIndex = -1;
    public int OldSelectedAccountIndex = -1;
    public int OldSelectedStreamIndex = -1;

    public List<Account> Accounts;
    public List<Branch> Branches;

    public Client Client;
    public Account SelectedAccount;
    public Stream SelectedStream;
    public List<Stream> Streams;

    public GameObject ConvertRecursivelyToNative(Base @base, string id)
    {

      var rc = GetComponent<RecursiveConverter>();
      if (rc == null)
        rc = gameObject.AddComponent<RecursiveConverter>();

      return rc.ConvertRecursivelyToNative(@base,
                                           Branches[SelectedBranchIndex].commits.items[SelectedCommitIndex].id);
    }
  }
}