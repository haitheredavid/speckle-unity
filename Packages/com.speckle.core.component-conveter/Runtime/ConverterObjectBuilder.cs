using System;
using System.Collections;
using System.Collections.Concurrent;
using Speckle.Core.Models;
using UnityEngine;
using Speckle.ConnectorUnity;
using UnityEngine.Events;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter
{


    public class ConverterObjectBuilder : MonoBehaviour
    {


        [SerializeField] private ConcurrentQueue<BuilderDataInput> queue;
        [SerializeField] Action<BuilderDataInput> buildAction;

        public bool isWorking {get;private set;}

        public bool isInit => buildAction != null && queue != null;

        public event Action<int> OnQueueSizeChange;
        public event Action OnStateChange;

        public void Initialize(Action<BuilderDataInput> unwrap)
        {
            this.buildAction = unwrap;
            this.queue = new ConcurrentQueue<BuilderDataInput>();
        }

        private Coroutine routine;

        public void AddToQueue(BuilderDataInput builderDataInput)
        {
            Debug.Log("Adding to queue");

            queue.Enqueue(builderDataInput);

            OnQueueSizeChange?.Invoke(queue.Count);
        }


        public void Update()
        {
            routine = queue.IsEmpty switch
            {
                false when routine == null => StartCoroutine(DoWork()),
                true when routine != null => null,
                _ => routine
            };
        }


        IEnumerator DoWork()
        {
            isWorking = true;
            while (queue.TryDequeue(out var item))
            {
                Debug.Log($"Doing work on {item}");
                buildAction.Invoke(item);
                yield return null;
            }

            OnQueueSizeChange?.Invoke(queue.Count);

            Debug.Log("All done!");
            isWorking = false;
        }



    }

}
