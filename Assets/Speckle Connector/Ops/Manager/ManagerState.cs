namespace ConnectorUnity
{
  public enum SpeckleManagerStates
  {
    AccountChanged = 0,
    StreamChanged = 1,
    BranchChanged = 2,
    CommitChanged = 3,
    KitChanged = 4
  }

  public enum ManagerState
  {
    Primed,
    AccountSelected,
    AccountLoaded,
    StreamSelected,
    BranchSelected,
    CommitSelected,
    KitSelected,
    NoAccounts,
    NoStreams,
    NoCommits,
    ClientError
  }
}