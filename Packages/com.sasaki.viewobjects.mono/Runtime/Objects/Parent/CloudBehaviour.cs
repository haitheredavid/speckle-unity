using System.Collections.Generic;
using System.Linq;
using Pcx;
using UnityEngine;
using ViewTo.Objects;
using ViewTo.Objects.Elements;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{

  [ExecuteAlways]
  public abstract class CloudBehaviour<TObj> : ViewObjBehaviour<TObj> where TObj : ViewCloud, new()
  {
    [ReadOnly] [SerializeField] protected int pointCount;
    [SerializeField] private PointCloudRenderer cloudRenderer;

    public string viewID
    {
      get => viewObj.viewID;
      set => viewObj.viewID = value;
    }

    public CloudPoint[] Points
    {
      get => viewObj.points;
      set
      {
        viewObj.points = value;
        pointCount = value.Length;
        UpdateRenderer();
      }
    }

    private void UpdateRenderer()
    {
      RenderPoints((from p in Points select p.ToUnity()).ToList(),
                   (from p in Points select Color.white).Select(dummy => (Color32)dummy).ToList());
    }

    protected override void ImportValidObj()
    {
      UpdateRenderer();
      gameObject.name = viewObj.TypeName();
    }

    protected void RenderPoints(List<Vector3> points, List<Color32> colors)
    {
      var data = ScriptableObject.CreateInstance<PointCloudData>();
      data.Initialize(points, colors);

      if (cloudRenderer == null)
      {
        cloudRenderer = new GameObject("CloudRender").AddComponent<PointCloudRenderer>();
        cloudRenderer.gameObject.transform.SetParent(transform);
      }

      cloudRenderer.sourceData = data;
    }
  }

}