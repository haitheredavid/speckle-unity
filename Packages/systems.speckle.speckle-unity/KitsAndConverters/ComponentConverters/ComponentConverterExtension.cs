using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  public static class ComponentConverterExtension
  {
    public static void SetupLineRenderer(this LineRenderer lineRenderer, Vector3[] points, float diameter = 1)
    {
      if (points.Length == 0) return;

      lineRenderer.positionCount = points.Length;
      lineRenderer.SetPositions(points);
      lineRenderer.numCornerVertices = lineRenderer.numCapVertices = 8;
      lineRenderer.startWidth = lineRenderer.endWidth = diameter;
    }

    public static Dictionary<string, object> FetchProps<TBase>(this TBase @base) where TBase : Base
    {
      var props = typeof(TBase).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(x => x.Name).ToList();
      return @base.GetMembers().Where(x => !props.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
    }

    public static void AttachUnityProperties(this Base @base, SpeckleProperties props)
    {
      if (@base == null || props?.Data == null)
        return;

      foreach (var key in props.Data.Keys)
        @base[key] = props.Data[key];
    }

  }
}