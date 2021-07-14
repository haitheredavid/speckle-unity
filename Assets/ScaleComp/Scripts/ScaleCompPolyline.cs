using System;
using System.Collections.Generic;
using Objects.Geometry;
using Speckle.Core.Models;
using UnityEngine;

namespace ScaleComp.Scripts

{

  [Serializable]
  public class ScaleCompBase<TBase> : Base where TBase : Base
  {

    public string compName;
    // Mapbox location 
    public string latitude, longitude;

    public float scale = 1f;

    // speckle reference
    public Base @Base { get; set; }

  }

  [Serializable]
  public class ScaleCompPolyline : ScaleCompBase<Polyline>
  {
    // field for unity editor
    public List<Vector3> segments;

  }
}