using System;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  public static class ConnectorConsole
  {

    public const string title = "speckle-unity:";

    public static void Log(string msg)
    {
      Debug.Log(title + msg);
    }
    public static void Exception(Exception exception)
    {
      Debug.LogException(exception);
    }
  }
}