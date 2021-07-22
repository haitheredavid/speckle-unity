using System.Collections.Generic;
using UnityEngine;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{

  public static partial class ViewConverter
  {

    public static Vector3[] ToUnity(this CloudPoint[] value)
    {
      var points = new Vector3[value.Length];
      for (int i = 0; i < value.Length; i++)
        points[i] = value[i].ToUnity();

      return points;
    }

    public static Vector3 ToUnity(this CloudPoint value)
    {
      return new Vector3((float) value.X, (float) value.Z, (float) value.Y);
    }

  }
}