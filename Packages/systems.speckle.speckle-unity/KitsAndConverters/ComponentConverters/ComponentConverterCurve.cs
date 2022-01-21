using Objects.Geometry;
using UnityEngine;

namespace Objects.Converter.Unity
{
  [CreateAssetMenu(fileName = "CurveConverter", menuName = "Speckle/Curve Converter")]
  public class ComponentConverterCurve : ComponentConverter<Curve>
  {
    public float diameter;

    /// <summary>
    ///   Converts a Speckle curve to a GameObject with a line renderer
    /// </summary>
    /// <param name="base"></param>
    /// <returns></returns>
    protected override Component Process(Curve @base)
    {
      var line = New<LineRenderer>(@base.speckle_type);
      line.SetupLineRenderer(ArrayToPoints(@base.points, @base.units), diameter);
      return line;
    }
  }
}