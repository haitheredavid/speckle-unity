using System;
using UnityEngine;

namespace ViewTo.Connector.Unity
{
  [Serializable]
  public class CloudShell
  {
    public int count;
    public string id;

    [HideInInspector] public string[] meta;
    [HideInInspector] public Vector3[] points;
  }
}