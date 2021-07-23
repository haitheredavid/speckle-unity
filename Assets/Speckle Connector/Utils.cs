using System;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  public static class Utils
  {

    public static void SafeDestroy(UnityEngine.Object obj)
    {
      if (Application.isPlaying)
        UnityEngine.Object.Destroy(obj);

      else
        UnityEngine.Object.DestroyImmediate(obj);

    }

    public static bool Valid(this string name) => !string.IsNullOrEmpty(name);

    public static Mesh SafeMeshGet(this MeshFilter mf) => Application.isPlaying ? mf.mesh : mf.sharedMesh;



    public static void SafeMeshSet(this GameObject go, Mesh m, bool addMeshFilterIfNotFound)
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


    public static void SafeMeshSet(this GameObject go, Mesh m)
    {
      var mf = go.GetComponent<MeshFilter>();
      if (mf == null) return;


      if (Application.isPlaying)
        mf.mesh = m;
      else
        mf.sharedMesh = m;
    }


    public static int ToIntColor(this Color c)
    {
      return
          System.Drawing.Color
              .FromArgb(Convert.ToInt32(c.r * 255), Convert.ToInt32(c.r * 255), Convert.ToInt32(c.r * 255))
              .ToArgb();
    }

    public static Color ToUnityColor(this int c)
    {
      var argb = System.Drawing.Color.FromArgb(c);
      return new Color(argb.R / 255.0f, argb.G / 255.0f, argb.B / 255.0f);
    }

    // ref from https://stackoverflow.com/questions/2692313/implementing-toargb
    public static Color32 ToUnityColor32(this int c)
    {
      // From integer 
      return new Color32((byte) (c >> 24),
                         (byte) (c >> 16),
                         (byte) (c >> 8),
                         (byte) (c));
    }

  }
}