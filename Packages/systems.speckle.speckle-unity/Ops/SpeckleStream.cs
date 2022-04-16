using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sentry;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  public class SpeckleStream : MonoBehaviour
  {

    [SerializeField] protected StreamShell shell;

    protected Client client;
    private bool isCanceled;

    public void Init(Account account, StreamShell streamShell)
    {
      client = new Client(account);

      // use either the params or the data stored here
      if (streamShell != null)
        shell = streamShell;
    }

    public async UniTask<Commit> GetCommit()
    {
      Commit commit = null;
      (isCanceled, commit) = await SpeckleConnector.GetCommit(client, shell).SuppressCancellationThrow();

      if (isCanceled || commit == null)
        Debug.LogWarning("The commit being asked for was not recieved correctly");

      return commit;
    }

  }
}