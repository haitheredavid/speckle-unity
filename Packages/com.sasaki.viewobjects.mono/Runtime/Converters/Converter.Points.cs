using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ViewTo.Objects.Elements;

namespace ViewTo.Connector.Unity
{

  public static partial class ViewConverter
  {

    public static Dictionary<string, CloudPoint[]> ToView(this List<CloudShell> objs)
    {
      return objs.ToDictionary(o => o.id, o => o.ToView());
    }
    
    public static CloudPoint[] ToView(this CloudShell value)
    {
      var items = new CloudPoint[value.count];

      for (int i = 0; i < value.count; i++)
        items[i] = value.points[i].ToView(value.meta[i]);

      return items;
    }

    public static CloudPoint[] ToView(Vector3[] value, string[] meta)
    {
      var items = new CloudPoint[value.Length];

      for (int i = 0; i < value.Length; i++)
        items[i] = value[i].ToView(meta[i]);

      return items;
    }

    public static CloudPoint ToView(this Vector3 value)
    {
      return new CloudPoint(value.x, value.z, value.y);
    }

    public static CloudPoint ToView(this Vector3 value, string meta)
    {
      return new CloudPoint(value.x, value.z, value.y) {meta = meta};
    }

    public static Vector3[] ToUnity(this CloudPoint[] value, out string[] meta)
    {
      meta = null;
      meta = new string[value.Length];
      var points = new Vector3[value.Length];
      for (var i = 0; i < value.Length; i++)
      {
        points[i] = value[i].ToUnity();
        meta[i] = value[i].meta;
      }

      return points;
    }
    public static Vector3[] ToUnity(this CloudPoint[] value)
    {
      var points = new Vector3[value.Length];
      for (var i = 0; i < value.Length; i++)
        points[i] = value[i].ToUnity();

      return points;
    }

    /// <summary>
    ///   flips value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Vector3 ToUnity(this CloudPoint value) => new Vector3((float)value.x, (float)value.z, (float)value.y);
  }
}