using System.Linq;
using UnityEngine;

namespace ViewTo.Connector.Unity
{

  public static partial class ViewFactory
  {

    public static void PrepContentMeshes(this ViewContentMono mono, Material material)
    {
      var mat = Object.Instantiate(new Material(material) {color = mono.ViewColor.ToUnity()});

      foreach (Transform child in mono.transform)
      {
        var co = child.gameObject.AddComponent<ContentObject>();
        co.CombineMeshes(mat);
      }
      mono.gameObject.SetLayerRecursively(mono.ContentMask);
    }

    public static void PrepAllContent(this ContentBundleMono mono, Material material = null)
    {
      if (material == null)
      {
        Debug.Log("Material is missing for view content prep, using default unlit");
        material = new Material(Shader.Find("Unlit/Color"));
      }

      var items = mono.GetAll;
      Debug.Log($"Prepping Case: {items.Count} ");
      foreach (var c in items)
        c.PrepContentMeshes(material);

      // SetupEvent?.Invoke(true);
    }

    // public static ContentBundleMono BuildCase(this ViewStudyMono studyMono)
    // {
    //   if (studyMono == null || !studyMono.IsReady) return null;
    //
    //
    //   return null;
    // }
  }
}