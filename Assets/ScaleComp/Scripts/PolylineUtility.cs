using System.Collections.Generic;
using Objects.Geometry;
using UnityEngine;

namespace ScaleComp.Scripts
{
  internal static class PolylineUtility
  {
    public static Polyline ToSpeckle(this IEnumerable<Vector2> segs) => new Polyline(segs.Format());
    public static Polyline ToSpeckle(this IEnumerable<Vector3> segs) => new Polyline(segs.Format());

    public static List<double> Format(this IEnumerable<Vector3> points)
    {
      var values = new List<double>();
      foreach (var p in points)
      {
        values.Add(p.x);
        values.Add(p.y);
        values.Add(p.z);
      }

      return values;
    }

    public static List<double> Format(this IEnumerable<Vector2> points)
    {
      var values = new List<double>();
      foreach (var p in points)
      {
        values.Add(p.x);
        values.Add(p.y);
        values.Add(0);
      }

      return values;
    }
  }
}