using System;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;

namespace ScaleComp.Scripts
{

  [ExecuteAlways]
  public class ScaleCompFactory : MonoBehaviour
  {

    [SerializeField] private AbstractMap map;
    [SerializeField] private ScaleComp comps ;

    public void Start()
    {
      comps = PrebuiltComps.Monterrey;
    }

    internal  class PrebuiltComps
    {

      public static readonly ScaleComp Monterrey = new ScaleComp
      {
        compName = "Monterrey", longitude = "-100.316116", latitude = "25.686613", segments = new List<Vector3>
        {
          new Vector3(-16.18f, 0f, 20.9f),
          new Vector3(-11.1f, 0f, 20.85f),
          new Vector3(-11.15f, 0f, 14.53f),
          new Vector3(-13.14f, 0f, 14.53f),
          new Vector3(-13.71f, 0f, 8.94f),
          new Vector3(-16.2f, 0f, 9.04f)
        }
      };

    }

  }
}