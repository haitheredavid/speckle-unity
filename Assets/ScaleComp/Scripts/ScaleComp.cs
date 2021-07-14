using System;
using System.Collections.Generic;
using Objects.Geometry;
using Speckle.Core.Models;
using UnityEngine;

namespace ScaleComp.Scripts

{
  [Serializable]
  public class ScaleCompBase : Base
  {
    // speckle reference
    public Polyline polyline { get;  set; }
    public string compName;
    public string latitude, longitude;

  }

  [Serializable]
  public class ScaleComp : ScaleCompBase
  {
    // field for unity editor
    public List<Vector3> segments;

  }
}