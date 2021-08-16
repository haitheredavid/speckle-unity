using System;
using System.Collections.Generic;
using ConnectorUnity.GUI;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;

namespace ConnectorUnity
{

  [Serializable]
  public class SpeckleConnectorInput
  {

    [DropDownString("Accounts")]
    public int account;
    [DropDownString("Streams")]
    public int stream;
    [DropDownString("Branches")] 
    public int branch;
    [DropDownString("Commits")]
    public int commit;
    [DropDownString("Kits")]
    public int kit;

    public void Set(IEnumerable<Account> value) => Accounts = value.Format();
    public void Set(IEnumerable<Stream> value) => Streams = value.Format();
    public void Set(IEnumerable<Branch> value) => Branches = value.Format();
    public void Set(IEnumerable<Commit> value) => Commits = value.Format();
    public void Set(IEnumerable<ISpeckleKit> value) => Kits = value.Format();

    public string[] Accounts, Streams, Branches, Commits, Kits;

  }
}