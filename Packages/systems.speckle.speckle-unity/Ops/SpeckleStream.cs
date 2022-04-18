using Cysharp.Threading.Tasks;
using Speckle.Core.Credentials;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  public class SpeckleStream : ScriptableObject
  {

    [SerializeField] private string serverUrl;
    [SerializeField] private string streamId;
    [SerializeField] private string branchName;
    [SerializeField] private string commitId;
    [SerializeField] private string objectId;
    [SerializeField] private string userId;

    [Space]
    [SerializeField] private bool expired;

    private bool isCanceled;

    public string UserId
    {
      get => userId;
    }

    public string ServerUrl
    {
      get => serverUrl;
    }

    public string StreamId
    {
      get => streamId;
    }

    public string CommitId
    {
      get => commitId;
    }

    public string BranchName
    {
      get => branchName;
    }

    public string ObjectId
    {
      get => objectId;
    }

    public bool Init(string streamUrlOrId)
    {
      ConnectorConsole.Log("Setting new Stream");

      var wrapper = new StreamWrapper(streamUrlOrId);

      if (!wrapper.IsValid)
      {
        ConnectorConsole.Log("Invalid input for stream");
        return false;
      }

      serverUrl = wrapper.ServerUrl;
      streamId = wrapper.StreamId;
      branchName = wrapper.BranchName;
      commitId = wrapper.CommitId;
      objectId = wrapper.ObjectId;
      userId = wrapper.UserId;

      return true;
    }

    public async UniTask<Account> GetAccount()
    {
      return await new StreamWrapper(streamId, userId, objectId).GetAccount();
    }

    public bool IsValid()
    {
      return new StreamWrapper(streamId, userId, serverUrl).IsValid;
    }

    public override string ToString()
    {
      return new StreamWrapper(streamId, userId, serverUrl).ToString();
    }

    // public async UniTask<Commit> GetCommit()
    // {
    //   Commit commit = null;
    //   (isCanceled, commit) = await SpeckleConnector.GetCommit(client, shell).SuppressCancellationThrow();
    //
    //   if (isCanceled || commit == null)
    //     Debug.LogWarning("The commit being asked for was not recieved correctly");
    //
    //   return commit;
    // }
  }
}