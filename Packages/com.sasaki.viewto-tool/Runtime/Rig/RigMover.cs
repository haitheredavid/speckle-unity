using UnityEngine;

namespace ViewToUnity
{
  public class RigMover
  {

    public Vector3[] ViewPoints { get; set; }

    public Vector3[] StoreViewPoints
    {
      set => ViewPoints = value;
    }

    public Vector3 NextPoint(int pointIndex) => ViewPoints[pointIndex];
  }
}