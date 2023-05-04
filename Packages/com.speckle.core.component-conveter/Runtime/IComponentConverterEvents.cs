using UnityEngine.Events;

namespace Speckle.ConnectorUnity.Converter
{

  public interface IComponentConverterEvents
  {
    public event UnityAction<int> OnQueueSizeChanged;

  }

}
