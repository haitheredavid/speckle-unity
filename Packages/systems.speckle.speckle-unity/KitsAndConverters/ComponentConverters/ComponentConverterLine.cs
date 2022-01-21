using System.Collections.Generic;
using Objects.Geometry;
using UnityEngine;

namespace Objects.Converter.Unity
{

  [CreateAssetMenu(fileName = "LineConverter", menuName = "Speckle/Line Converter")]
  public class ComponentConverterLine : ComponentConverter<Line>
  {
    public float diameter;

    protected override Component Process(Line @base)
    {
      var line = New<LineRenderer>(@base.speckle_type);
      line.SetupLineRenderer(new[] { ToVector3(@base.start), ToVector3(@base.end) }, diameter);
      return line;
    }

  }
}