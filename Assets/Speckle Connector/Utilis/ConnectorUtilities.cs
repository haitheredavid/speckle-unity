using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ConnectorUnity
{

  public static class ConnectorUtilities
  {

    public static void SafeDestroy(UnityEngine.Object obj)
    {
      if (Application.isPlaying)
        UnityEngine.Object.Destroy(obj);

      else
        UnityEngine.Object.DestroyImmediate(obj);

    }

    // ref from https://stackoverflow.com/questions/2692313/implementing-toargb

  }
}