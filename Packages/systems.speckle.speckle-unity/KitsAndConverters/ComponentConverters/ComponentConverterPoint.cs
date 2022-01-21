using Objects.Geometry;
using UnityEngine;

namespace Objects.Converter.Unity
{
  [CreateAssetMenu(fileName = "PointConverter", menuName = "Speckle/Point Converter")]
  public class ComponentConverterPoint : ComponentConverter<Point>
  {
    public float diameter;

    protected override Component Process(Point @base)
    {
      var ptn = New<LineRenderer>(@base.speckle_type);
      ptn.SetupLineRenderer(new[] { ToVector3(@base) }, diameter);
      return ptn;
    }
  }
}