using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using Speckle.Core.Transports;
using UnityEngine;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter.Scenes
{

    public class BasicConversions : MonoBehaviour
    {


        [SerializeField]
        ScriptableConverter converter;

        [SerializeField]
        int queueSize = 10;
        
        [SerializeField]
        string streamId = "a823053e07";
        [SerializeField]
        string commitId = "";
        [SerializeField]
        string objectId = "c4fb5e504ba7299fec99607574b06572";

        [SerializeField]
        Transform parent;

        private CancellationTokenSource source;

        private void Awake()
        {
            source = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            if (source.IsCancellationRequested) source.Token.ThrowIfCancellationRequested();
            source.Dispose();
        }

        async void Start()
        {
            Speckle.Core.Logging.SpeckleLog.Initialize(HostApplications.Unity.Name, HostApplications.Unity.GetVersion(HostAppVersion.v2021));

            var items = new List<Base>();
            for (int i = 0; i < queueSize; i++)
            {
                var b = await Run();
                items.Add(b);
            }
            Debug.Log("Done with Recieve");

            foreach (var i in items)
            {
                converter.ConvertToNative(i);
            }
        }

        async Task<Base> Run()
        {
            var client = new Client(AccountManager.GetDefaultAccount());
            var c = await client.ObjectGet(streamId, objectId);
            var obj = await Operations.Receive(c.id, new ServerTransport(client.Account, streamId));
            return obj;
        }




    }

}
