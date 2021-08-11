using UnityEngine;

namespace ViewTo.Connector.Unity
{
  public static class GoExtension
  {

    public static TComp GetOrSet<TComp>(this GameObject go) where TComp : MonoBehaviour
    {
      var comp = go.GetComponent<TComp>();
      if (comp == null)
        comp = go.AddComponent<TComp>();

      return comp;
    }
  }
}