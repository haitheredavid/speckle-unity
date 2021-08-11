using Speckle.Core.Api;
using Speckle.Core.Credentials;
using System.Collections.Generic;
using System.Linq;
using Objects.Converter.Unity;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  [ExecuteAlways]
  public class StreamManager : MonoBehaviour
  {

    public int SelectedAccountIndex = -1;
    public int SelectedStreamIndex = -1;
    public int SelectedBranchIndex = -1;
    public int SelectedCommitIndex = -1;
    public int OldSelectedAccountIndex = -1;
    public int OldSelectedStreamIndex = -1;

    public Client Client;
    public Account SelectedAccount;
    public Stream SelectedStream;

    public List<Account> Accounts;
    public List<Stream> Streams;
    public List<Branch> Branches;

    public int SelectedKitIndex = -1;
    public int OldSelectedKitIndex = -1;

    private List<ISpeckleKit> _kits;

    [SerializeField] private bool kitsInit;
    [SerializeField] private bool useDefaultConverter;

    private ISpeckleConverter _converter;

    public List<string> GetKits
    {
      get
      {
        if (!kitsInit || _kits == null)
          _kits ??= KitManager.Kits.ToList();

        kitsInit = _kits != null;

        return _kits != null ? _kits.Select(kit => kit.Name).ToList() : new List<string>();
      }
    }
    // private ViewObjBaseConverter _converter = new ViewObjBaseConverter();

    public GameObject ConvertRecursivelyToNative(Base @base, string id)
    {
      // _converter ??= new ViewObjBaseConverter();
      if (useDefaultConverter)
      {
        Debug.Log("Using Default Converter");

        var rc = GetComponent<RecursiveConverter>();
        if (rc == null)
          rc = gameObject.AddComponent<RecursiveConverter>();
        return rc.ConvertRecursivelyToNative(@base, Branches[SelectedBranchIndex].commits.items[SelectedCommitIndex].id);
      }

      var o = _converter.ConvertToNative(@base);

      if (o is ICanMono mono)
        return mono.BuildMono();

      return null;
    }

    public void LoadKit()
    {
      if (_kits != null && _kits.Count > SelectedKitIndex)
      {
        _converter = _kits[SelectedKitIndex].LoadConverter(Applications.Unity);

        if (_converter != null)
          Debug.Log($"Converter is Loaded {_converter.Name}");

        else
          Debug.Log("Converter is loaded");

      }

    }
  }
}