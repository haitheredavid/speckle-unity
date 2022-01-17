using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Speckle.ConnectorUnity
{
  public static class Utils
  {

    public static void SafeDestroy(Object obj)
    {
      if (Application.isPlaying)
        Object.Destroy(obj);

      else
        Object.DestroyImmediate(obj);

    }

    public static bool Valid(this string name) => !string.IsNullOrEmpty(name);

    public static Mesh SafeMeshGet(this MeshFilter mf) => Application.isPlaying ? mf.mesh : mf.sharedMesh;

    public static void SafeMeshSet(this GameObject go, Mesh m, bool addMeshFilterIfNotFound = true)
    {

      var mf = go.GetComponent<MeshFilter>();
      if (mf == null)
      {
        if (!addMeshFilterIfNotFound) return;

        mf = go.AddComponent<MeshFilter>();
      }


      if (Application.isPlaying)
        mf.mesh = m;
      else
        mf.sharedMesh = m;
    }

    public static int ToIntColor(this Color c) => System.Drawing.Color
      .FromArgb(Convert.ToInt32(c.r * 255), Convert.ToInt32(c.r * 255), Convert.ToInt32(c.r * 255))
      .ToArgb();

    public static Color ToUnityColor(this int c)
    {
      var argb = System.Drawing.Color.FromArgb(c);
      return new Color(argb.R / 255.0f, argb.G / 255.0f, argb.B / 255.0f);
    }
  }
}