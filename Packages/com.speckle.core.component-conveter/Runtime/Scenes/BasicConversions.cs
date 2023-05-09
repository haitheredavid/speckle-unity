using System;
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
        string stream, commit;

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
            var b = await Run();
            var instance = new ConverterInstance(converter, b, source.Token);
            instance.Run();

        }

        async Task<Base> Run()
        {
            Debug.Log("Is Running");
            var client = new Client(AccountManager.GetDefaultAccount());
            var c = await client.CommitGet(stream, commit);
            Debug.Log("Commit recieved");
            var obj = await Operations.Receive(c.referencedObject, new ServerTransport(client.Account, stream));
            if (obj != null) Debug.Log("Success!");
            return obj;
        }



    }

}
