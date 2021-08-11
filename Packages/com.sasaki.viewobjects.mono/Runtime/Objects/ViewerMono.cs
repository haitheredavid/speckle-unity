using UnityEngine;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{

  public class ViewerMono : MonoBehaviour
  {

    public void Setup(Viewer viewer) => Align(viewer.Direction);

    private void Align(ViewerDirection dir)
    {
      var camDirection = dir switch
      {
        ViewerDirection.Front => new Vector3(0, 0, 0),
        ViewerDirection.Left => new Vector3(0, 90, 0),
        ViewerDirection.Back => new Vector3(0, 180, 0),
        ViewerDirection.Right => new Vector3(0, -90, 0),
        ViewerDirection.Up => new Vector3(90, 0, 0),
        ViewerDirection.Down => new Vector3(-90, 0, 0),
        _ => new Vector3(0, 0, 0)
      };
      transform.localRotation = Quaternion.Euler(camDirection);
    }
  }
}