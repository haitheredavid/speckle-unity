using System;
using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;

namespace ConnectorUnity
{
  [Serializable]
  public struct SpeckleManagerCache
  {

    public SpeckleManagerCache(int startValue = 0)
    {
      account = startValue;
      stream = startValue;
      branch = startValue;
      commit = startValue;
      kit = startValue;

      Accounts = new List<Account>();
      Streams = new List<Stream>();
      Branches = new List<Branch>();
      Commits = new List<Commit>();
      Kits = new List<ISpeckleKit>();
    }

    
    public int account;
    public int stream;
    public int branch;
    public int commit;
    public int kit;

    public List<Account> Accounts { get; set; }
    public List<Stream> Streams { get; set; }
    public List<Branch> Branches { get; set; }
    public List<Commit> Commits { get; set; }
    public List<ISpeckleKit> Kits { get; set; }



    /// <summary>
    /// Helper method for passing editor input data from the Speckle Input Manager  
    /// </summary>
    /// <param name="input">Values from editor to store</param>
    public List<SpeckleManagerStates> UpdateAndCheck(ManagerData input)
    {
      var states = new List<SpeckleManagerStates>();

      if (account != input.account)
        states.Add(SpeckleManagerStates.AccountChanged);

      else if (stream != input.stream)
        states.Add(SpeckleManagerStates.StreamChanged);

      else if (branch != input.branch)
        states.Add(SpeckleManagerStates.BranchChanged);

      else if (commit != input.commit)
        states.Add(SpeckleManagerStates.CommitChanged);

      else if (kit != input.kit)
        states.Add(SpeckleManagerStates.KitChanged);

      account = input.account;
      stream = input.stream;
      branch = input.branch;
      commit = input.commit;
      kit = input.kit;

      return states;
    }
  }
}