using System;
using System.Collections.Generic;
using System.Linq;
using Objects.Geometry;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;

namespace Objects.Converter.Unity
{
  public abstract class ComponentConverter<TBase> : ScriptableObject where TBase : Base
  {
    public bool storeProps = true;

    protected abstract Component Process(TBase @base);

    public Component ToComponent(TBase @base)
    {
      var comp = Process(@base);

      if (storeProps)
        comp.gameObject.AddProps(@base);

      return comp;
    }

    // TODO: Handle list and nested objects
    // public Component ToComponent(IEnumerable<TBase> @base)
    // {
    //   
    //   var comp = Process(@base);
    //
    //   if (storeProps)
    //     comp.gameObject.AddProps(@base);
    //
    //   return comp;
    // }
    //

    /// <summary>
    /// Check to see if converter is processing during editor or playing mode
    /// </summary>
    protected bool IsRuntime
    {
      get => Application.isPlaying;
    }

    protected static TBehave New<TBehave>(string name = "@base") where TBehave : Component
    {
      return new GameObject(name).AddComponent<TBehave>();
    }

    #region static helpers
    /// <summary>
    ///   the default Unity units are meters
    /// </summary>
    public static string ModelUnits = Units.Meters;

    public static double ScaleToNative(double value, string units)
    {
      return value * Units.GetConversionFactor(units, ModelUnits);
    }

    /// <summary>
    /// switch y and z
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="units"></param>
    /// <returns></returns>
    public static Vector3 VectorByCoordinates(double x, double y, double z, string units)
    {
      return new Vector3((float)ScaleToNative(x, units), (float)ScaleToNative(z, units), (float)ScaleToNative(y, units));
    }

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

    /// <summary>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="units"></param>
    /// <param name="flipYZ"></param>
    /// <returns></returns>
    protected static Vector3 ToVector3(double x, double y, double z, string units, bool flipYZ = true)
    {
      // switch y and z
      return flipYZ ?
        new Vector3(
          (float)ScaleToNative(x, units),
          (float)ScaleToNative(z, units),
          (float)ScaleToNative(y, units)) :
        new Vector3(
          (float)ScaleToNative(x, units),
          (float)ScaleToNative(y, units),
          (float)ScaleToNative(z, units));
    }

    protected static Vector3 ToVector3(Point p, bool flipYZ = true)
    {
      return flipYZ ?
        new Vector3(
          (float)ScaleToNative(p.x, p.units),
          (float)ScaleToNative(p.z, p.units),
          (float)ScaleToNative(p.y, p.units))
        :
        new Vector3(
          (float)ScaleToNative(p.x, p.units),
          (float)ScaleToNative(p.y, p.units),
          (float)ScaleToNative(p.z, p.units));
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
    #endregion

  }
}