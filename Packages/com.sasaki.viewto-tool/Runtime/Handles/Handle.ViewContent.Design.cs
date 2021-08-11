using UnityEngine;

namespace ViewTo.Connector.Unity
{
  public static class Handle
  {

    public static void DisplayMeshes(this DesignContentMono mono, bool value)
    {
      Debug.Log($"Setting {mono.gameObject.name} visibility to {value}");
      mono.gameObject.SetMeshVisibilityRecursive(value);
    }
  }
}