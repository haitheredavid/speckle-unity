using System.Collections.Generic;
using UnityEngine;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{

  public static partial class ViewFactory
  {
    public static Texture2D DrawPixelLine(this List<Color32> c, bool readAlpha = false)
    {
      var tempTexture = new Texture2D(c.Count, 1);

      for (var x = 0; x < tempTexture.width; x++)
      {
        var temp = !readAlpha ? new Color32(c[x].r, c[x].g, c[x].b, 255) : new Color32(c[x].r, c[x].g, c[x].b, c[x].a);

        tempTexture.SetPixel(x, 0, temp);
      }
      tempTexture.Apply();
      return tempTexture;
    }

    public static void SetupLayout(this ViewerBundleMono mono)
    {
      mono.viewers = new List<ViewerMono>();
      foreach (var layout in mono.layouts)
      {
        var prefab = new GameObject().AddComponent<ViewerMono>();
        foreach (var viewer in layout.viewers)
        {
          var vc = Object.Instantiate(prefab, mono.transform);
          vc.name = mono.name + viewer.Direction.TypeName();
          vc.Setup(viewer);
        }
      }
    }

    public static void CreateCam(this ViewerMono mono)
    {
      var cam = mono.gameObject.GetOrSet<ViewCamera>();
      cam.CamDistance = 100000;
      cam.transform.SetPositionAndRotation(mono.transform.position, mono.transform.rotation);
      cam.transform.SetParent(mono.transform);
    }
  }
}