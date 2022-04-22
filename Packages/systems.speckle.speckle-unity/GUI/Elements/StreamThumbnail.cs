using UnityEngine.UIElements;

namespace Speckle.ConnectorUnity.GUI
{

  public class StreamThumbnail : VisualElement
  {

    public Image thumbnail;

    public StreamThumbnail()
    {
      thumbnail = new Image();
      Add(thumbnail);
    }
  }
}