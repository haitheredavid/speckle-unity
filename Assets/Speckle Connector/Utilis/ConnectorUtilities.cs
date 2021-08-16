using UnityEngine;

namespace ConnectorUnity
{

  public static class ConnectorUtilities
  {

    public static void SafeDestroy(Object obj)
    {
      if (Application.isPlaying)
        Object.Destroy(obj);

      else
        Object.DestroyImmediate(obj);

    }
  }
}