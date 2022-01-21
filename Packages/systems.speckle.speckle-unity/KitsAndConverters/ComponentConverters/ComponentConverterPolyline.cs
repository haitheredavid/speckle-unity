using System.Linq;
using Objects.Geometry;
using UnityEngine;

namespace Objects.Converter.Unity
{
  [CreateAssetMenu(fileName = "PolylineConverter", menuName = "Speckle/Polyline Converter")]
  public class ComponentConverterPolyline : ComponentConverter<Polyline>
  {

    public float diameter;

    /// <summary>
    ///   Converts a Speckle polyline to a GameObject with a line renderer
    /// </summary>
    /// <param name="base"></param>
    /// <returns></returns>
    protected override Component Process(Polyline @base)
    {
      var line = New<LineRenderer>(@base.speckle_type);
      line.SetupLineRenderer(@base.points.Select(x => ToVector3(x)).ToArray(), diameter);
      return line;
    }
  }
}