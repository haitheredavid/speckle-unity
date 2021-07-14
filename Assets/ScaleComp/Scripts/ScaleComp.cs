using UnityEngine;

namespace ScaleComp.Scripts
{
  public class ScaleComp : MonoBehaviour
  {
    [SerializeField] private ScaleCompPolyline comp;

    public string Location => string.Join(comp.latitude, comp.longitude);

    // TODO: set scale from speckle props and current map zoom level
    public Vector3 Scale => Vector3.one;

    public ScaleCompPolyline Comp
    {
      get => comp;
      set => comp = value;
    }
  }
}