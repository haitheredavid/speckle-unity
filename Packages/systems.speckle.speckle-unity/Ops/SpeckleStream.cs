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

    private StreamWrapper _wrapper;

    public StreamWrapper Wrapper
    {
      get { return _wrapper ??= new StreamWrapper(originalInput); }
    }

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
      get => Wrapper.Type;
    }

    public bool Init()
    {
      ConnectorConsole.Log($"Setting new Stream with {originalInput}");

      _wrapper = new StreamWrapper(originalInput);

      if (!_wrapper.IsValid)
      {
        ConnectorConsole.Log("Invalid input for stream");
        return false;
      }

      serverUrl = _wrapper.ServerUrl;
      streamId = _wrapper.StreamId;
      branchName = _wrapper.BranchName;
      commitId = _wrapper.CommitId;
      objectId = _wrapper.ObjectId;
      userId = _wrapper.UserId;
      originalInput = _wrapper.OriginalInput;

      ConnectorConsole.Log(_wrapper.ToString());
      return true;
    }

    public bool Init(string streamUrlOrId)
    {
      originalInput = streamUrlOrId;
      return Init();
    }

    public async UniTask<Account> GetAccount()
    {
      return await Wrapper.GetAccount();
    }

    public bool IsValid()
    {
      return Wrapper.IsValid;
    }

    public override string ToString()
    {
      return Wrapper.ToString();
    }
  }
}