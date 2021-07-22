using System;
using System.Collections.Generic;
using System.Linq;
using Pcx;
using UnityEngine;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{

  [ExecuteAlways]
  public abstract class CloudBehaviour<TObj> : ViewObjBehaviour<TObj> where TObj : ViewCloud
  {

    [SerializeField] protected int pointCount = 0;
    [SerializeField] private PointCloudRenderer cloudRenderer;
    private CloudPoint[] _cloudPoints;

    public virtual event Action<bool> SetupEvent;

    public CloudPoint[] Points
    {
      get => _cloudPoints;
      protected set
      {
        // TODO: Throw error for invalid value
        if (viewObj == null || value == null)
        {
          SetupEvent?.Invoke(false);
          return;
        }

        _cloudPoints = value;
        pointCount = value.Length;
        RenderPoints((from p in value select p.ToUnity()).ToList());
        SetupEvent?.Invoke(true);
      }
    }

    protected override void ImportValidObj()
    {
      Points = viewObj.Points;
    }

    protected void RenderPoints(List<Vector3> points)
    {
      var colors = (from p in points select Color.white).Select(dummy => (Color32) dummy).ToList();
      RenderPoints(points, colors);
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

    public bool IsReady { get; private set; }

    private void Awake()
    {
      SetComponentElements();
    }

    private void SetComponentElements()
    {
      SetupEvent += b =>
      {
        {
          Debug.Log($"Setup Update: {gameObject.name} is ready? {b} ");
          IsReady = b;
        }
      };
    }

  }

}