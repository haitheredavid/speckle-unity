using System;
using Cysharp.Threading.Tasks;
using Speckle.Core.Api;
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

    private Client client;
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

    public async UniTask<bool> Init(string streamUrlOrId)
    {
      ConnectorConsole.Log("Setting new Stream");

      var wrapper = new StreamWrapper(streamUrlOrId);

      if (!wrapper.IsValid)
      {
        ConnectorConsole.Log("Invalid input for stream");
        return false;
      }

      try
      {
        var account = await wrapper.GetAccount();

        if (account != null)
        {
          client = new Client(account);
          serverUrl = wrapper.ServerUrl;
          streamId = wrapper.StreamId;
          branchName = wrapper.BranchName;
          commitId = wrapper.CommitId;
          objectId = wrapper.ObjectId;
          userId = wrapper.UserId;
        }
      }
      catch (Exception e)
      {
        ConnectorConsole.Exception(e);
      }

      return client != null;
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