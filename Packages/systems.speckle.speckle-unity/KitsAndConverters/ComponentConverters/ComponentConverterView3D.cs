using Objects.BuiltElements;
using UnityEngine;

namespace Objects.Converter.Unity
{
  [CreateAssetMenu(fileName = "View3dConverter", menuName = "Speckle/View3d Converter")]
  public class ComponentConverterView3D : ComponentConverter<View3D>
  {

    /// <summary>
    ///  Converts a Speckle View3D to a GameObject
    /// </summary>
    /// <param name="base"></param>
    /// <returns></returns>
    protected override Component Process(View3D @base)
    {
      var comp = New<Camera>(@base.name);

      comp.transform.position = VectorByCoordinates(
        @base.origin.x, @base.origin.y, @base.origin.z, @base.origin.units);

      comp.transform.forward = VectorByCoordinates(
        @base.forwardDirection.x, @base.forwardDirection.y, @base.forwardDirection.z, @base.forwardDirection.units);

      comp.transform.up = VectorByCoordinates(
        @base.upDirection.x, @base.upDirection.y, @base.upDirection.z, @base.upDirection.units);

      return comp;
    }
  }
}