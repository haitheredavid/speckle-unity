using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;

namespace Objects.Converter.Unity
{
  public class ConverterComponent : ScriptableObject
  {

    /// <summary>
    ///   the default Unity units are meters
    /// </summary>
    public static string ModelUnits = Units.Meters;

    public static double ScaleToNative(double value, string units)
    {
      var f = Units.GetConversionFactor(units, ModelUnits);
      return value * f;
    }
    /// <summary>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="units"></param>
    /// <returns></returns>
    public static Vector3 VectorByCoordinates(double x, double y, double z, string units) =>
      // switch y and z
      new Vector3((float)ScaleToNative(x, units), (float)ScaleToNative(z, units), (float)ScaleToNative(y, units));

    public static Vector3[] ArrayToPoints(IEnumerable<double> arr, string units)
    {
      if (arr == null)
        throw new Exception("point array is not valid ");

      if (arr.Count() % 3 != 0)
        throw new Exception("Array malformed: length%3 != 0.");

      var points = new Vector3[arr.Count() / 3];
      var asArray = arr.ToArray();
      for (int i = 2, k = 0; i < arr.Count(); i += 3)
        points[k++] = VectorByCoordinates(asArray[i - 2], asArray[i - 1], asArray[i], units);


      return points;
    }

    protected static Dictionary<string, object> FetchProps<TBase>(TBase @base) where TBase : Base
    {
      var props = typeof(TBase).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(x => x.Name).ToList();
      return @base.GetMembers().Where(x => !props.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
    }

    protected static IEnumerable<Vector2> GenerateUV(IReadOnlyList<Vector3> verts, float xSize, float ySize)
    {
      var uv = new Vector2[verts.Count];
      for (var i = 0; i < verts.Count; i++)
      {

        var vert = verts[i];
        uv[i] = new Vector2(vert.x / xSize, vert.y / ySize);
      }
      return uv;
    }
  }
}