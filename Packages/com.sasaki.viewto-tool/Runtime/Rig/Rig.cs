using System.Collections.Generic;
using UnityEngine;

namespace ViewTo.Connector.Unity
{
  public class Rig : MonoBehaviour
  {
    // [Header("|| Params ||")]
    // [ReadOnly] [SerializeField] private List<CloudShell> points;
    // [ReadOnly] [SerializeField] private int totalBundleCount;

    [Header("|| Runtime||")]
    [SerializeField] [Range(0, 300)] private int frameRate = 180;
    [SerializeField] private bool isRunning;
  }

}