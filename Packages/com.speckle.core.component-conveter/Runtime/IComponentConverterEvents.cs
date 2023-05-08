using UnityEngine.Events;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter
{

  public interface IComponentConverterEvents
  {
    public event UnityAction<int> OnQueueSizeChanged;

  }

}
