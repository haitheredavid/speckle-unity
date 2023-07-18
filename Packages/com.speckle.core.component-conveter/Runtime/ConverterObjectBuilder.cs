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

        private Coroutine _routine;

        /// <summary>
        /// returns true if there are objects in the queue and the action routine is in process
        /// </summary>
        public bool isWorking => queue.Valid() && _routine != null;

        /// <summary>
        /// returns true if the queue is setup and the action serialized 
        /// </summary>
        public bool isInit => buildAction != null && queue != null;

        public event Action<int> OnQueueSizeChange;
        public event Action OnStateChange;


        public void Initialize(Action<BuilderDataInput> unwrap)
        {
            buildAction = unwrap;
            queue = new ConcurrentQueue<BuilderDataInput>();
        }

        public void AddToQueue(BuilderDataInput builderDataInput)
        {
            Debug.Log("Adding to queue");

            queue.Enqueue(builderDataInput);

            OnQueueSizeChange?.Invoke(queue.Count);
        }

        void Update()
        {
            _routine = queue.IsEmpty switch
            {
                false when _routine == null => StartCoroutine(DoWork()),
                true when _routine != null => null,
                _ => _routine
            };
        }

        private void OnDestroy()
        {
            if (_routine != null) StopCoroutine(_routine);
        }

        IEnumerator DoWork()
        {
            while (queue.TryDequeue(out var item))
            {
                Debug.Log($"Doing work on {item}");
                buildAction.Invoke(item);
                yield return null;
            }

            OnQueueSizeChange?.Invoke(queue.Count);
            Debug.Log("All done!");
        }



    }

}
