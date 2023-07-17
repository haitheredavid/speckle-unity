using System;
using UnityEngine.Events;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter
{

  public interface IHaveAQueue
  {
      public bool isWorking {get;}

    public event Action<int> OnQueueSizeChanged;

  }

}
