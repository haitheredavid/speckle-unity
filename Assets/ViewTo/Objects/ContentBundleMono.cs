using System;
using System.Collections.Generic;
using System.Linq;
using HaiThere.Utilities;
using UnityEngine;
using ViewTo.Objects;
using ViewTo.Structure;

namespace ViewTo.Connector.Unity
{

  public class ContentBundleMono : ViewObjBehaviour<ContentBundle>
  {

    [SerializeField] private List<TargetContentMono> Targets;
    [SerializeField] private List<BlockerContentMono> Blockers;
    [SerializeField] private List<DesignContentMono> Designs;

    public bool IsReady { get; private set; }
    public event Action<bool> SetupEvent;

    public IEnumerable<IViewContentMono> Contents
    {
      get
      {
        var items = new List<IViewContentMono>();
        if (Targets != null) items.AddRange(Targets);
        if (Blockers != null) items.AddRange(Blockers);
        if (Designs != null) items.AddRange(Designs);
        return items;
      }
    }

    public void SetContent<TContentMono, TContent>(TContentMono content) 
      where TContent : ViewContent
      where TContentMono : ViewContentMono<TContent>
    {
      switch (content)
      {
        case TargetContentMono c:
          Targets ??= new List<TargetContentMono>();
          Targets.Add(c);
          break;
        case BlockerContentMono c:
          Blockers ??= new List<BlockerContentMono>();
          Blockers.Add(c);
          break;
        case DesignContentMono c:
          Designs ??= new List<DesignContentMono>();
          Designs.Add(c);
          break;
        default:
          Debug.Log($"Type {content.TypeName()} is not supported");
          break;
      }

    }

    public void PrepAllContent(Material material = null)
    {
      if (material == null)
      {
        Debug.Log("Material is missing for view content prep, using default unlit");
        material = new Material(Shader.Find("Unlit/Color"));
      }

      Debug.Log($"Prepping Case: {Contents.Count()} ");
      foreach (var c in Contents)
      {
        c.PrepContentMeshes(material);
      }

      SetupEvent?.Invoke(true);
    }

    public void PurgeAllContent()
    {
      var c = Contents.ToArray();
      if (c.Valid())
        for (int i = c.Length - 1; i > 0; i--)
          c[i].DestroySelf();

      Targets = new List<TargetContentMono>();
      Blockers = new List<BlockerContentMono>();
      Designs = new List<DesignContentMono>();

    }

    protected override void ImportValidObj()
    {
      PurgeAllContent();
      if (viewObj.Targets != null) Targets = AddToScene<TargetContentMono>(viewObj.Targets);
      if (viewObj.Blockers != null) Blockers = AddToScene<BlockerContentMono>(viewObj.Blockers);
      if (viewObj.Designs != null) Designs = AddToScene<DesignContentMono>(viewObj.Designs);
    }

    private List<TShell> AddToScene<TShell>(IEnumerable<ViewContent> objs) where TShell : ViewObjBehaviour
      => objs.Select(o => ComponentMil.Build<TShell>(o)).Where(item => item != null).ToList();

    private void Awake()
    {
      SetComponentElements();
    }

    private void SetComponentElements()
    {
      SetupEvent += b =>
      {
        {
          IsReady = b;
        }
      };
    }

  }
}