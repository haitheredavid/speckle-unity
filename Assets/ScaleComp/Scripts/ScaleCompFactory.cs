using System;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Objects.Geometry;
using UnityEngine;

namespace ScaleComp.Scripts
{

  [ExecuteAlways]
  public class ScaleCompFactory : MonoBehaviour
  {

    [SerializeField] private AbstractMap map;
    [SerializeField] private List<ScaleComp> comps;

    private void Start()
    {
      comps = new List<ScaleComp>();
      SetComp(PrebuiltComps.Monterrey);
    }

    private void Update()
    {
      foreach (var sc in comps)
      {
        sc.transform.localPosition = map.GeoToWorldPosition(Conversions.StringToLatLon(sc.Location));
        sc.transform.localScale = sc.Scale;
      }
    }

    private void SetComp(ScaleComp sc)
    {
      if (map != null && sc != null)
      {
        var instance = Instantiate(sc);
        instance.transform.localPosition = map.GeoToWorldPosition(Conversions.StringToLatLon(sc.Location));
        instance.transform.localScale = sc.Scale;

        comps ??= new List<ScaleComp>();
        comps.Add(instance);
      }
    }

    internal class PrebuiltComps
    {

      public static ScaleComp Monterrey
      {
        get
        {
          var sc = new GameObject("SC-Monterrey").AddComponent<ScaleComp>();
          sc.Comp = SpeckleComps.Monterrey;
          return sc;
        }
      }

      public static class SpeckleComps
      {
        public static ScaleCompPolyline Monterrey
        {
          get
          {
            var sc = new ScaleCompPolyline
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
            sc.Base = new Polyline(sc.segments.Format());
            return sc;
          }
        }
      }

    }

  }
}