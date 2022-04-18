using Cysharp.Threading.Tasks;
using Speckle.Core.Credentials;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  [CreateAssetMenu(menuName = "Speckle/Create Speckle Stream Object", fileName = "SpeckleStream", order = 0)]
  public class SpeckleStream : ScriptableObject
  {

    [SerializeField] private string serverUrl;
    [SerializeField] private string streamId;
    [SerializeField] private string branchName;
    [SerializeField] private string commitId;
    [SerializeField] private string objectId;
    [SerializeField] private string userId;

    [SerializeField] private string originalInput;

    private StreamWrapper wrapper;

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

    public string OriginalInput
    {
      get => originalInput;
    }

    public StreamWrapperType Type
    {
      get => new StreamWrapper(originalInput).Type;
    }

    public bool Init(string streamUrlOrId)
    {
      ConnectorConsole.Log("Setting new Stream");

      wrapper = new StreamWrapper(streamUrlOrId);

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
      originalInput = wrapper.OriginalInput;

      return true;
    }

    public async UniTask<Account> GetAccount()
    {
      wrapper ??= new StreamWrapper(originalInput);
      return await wrapper.GetAccount();
    }

    public bool IsValid()
    {
      wrapper ??= new StreamWrapper(originalInput);
      return wrapper.IsValid;
    }

    public override string ToString()
    {
      wrapper ??= new StreamWrapper(originalInput);
      return wrapper.ToString();
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