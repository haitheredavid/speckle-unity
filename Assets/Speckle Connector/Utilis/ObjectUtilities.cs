using System;
using System.Collections;
using UnityEngine;

namespace ConnectorUnity
{
  public static class ObjectUtilities
  {
    public static bool Valid(this string name) => !string.IsNullOrEmpty(name);

    public static bool Valid(this IList list) => list != null && list.Count > 0;

    public static bool Valid(this ICollection list) => list != null && list.Count > 0;

    public static Mesh SafeMeshGet(this MeshFilter mf) => Application.isPlaying ? mf.mesh : mf.sharedMesh;


    public static void SafeMeshSet(this GameObject go, Mesh m)
    {
      var mf = go.GetComponent<MeshFilter>();
      if (mf == null)
        mf = go.AddComponent<MeshFilter>();

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

    public static Color32 ToUnityColor32(this int c)
    {
      // From integer 
      return new Color32((byte)(c >> 24),
                         (byte)(c >> 16),
                         (byte)(c >> 8),
                         (byte)(c));
    }

  }
}