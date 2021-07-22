using System.Collections.Generic;
using HaiThere.Utilities;
using UnityEngine;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{

  public class ViewerBundleMono : ViewObjBehaviour<ViewerBundle>
  {

    [SerializeField] private List<ViewerComponent> _viewers;
    [SerializeField] private Texture2D colorStrip;
    [SerializeField] private List<string> clouds;
    [SerializeField] private bool global;
    [SerializeField] private List<Color32> colors;

    public List<MetaShell> LinkedShell { get; private set; }
    public int ViewerCount { get; private set; }

    public bool IsGlobal
    {
      get => global;
      set => global = value;
    }

    protected override void ImportValidObj()
    {
      gameObject.name = viewObj.TypeName();
      ViewerCount = viewObj.layouts.Count;
      if (viewObj is ViewerBundleLinked linked && linked.linkedClouds.Valid())
        LinkedShell = linked.linkedClouds;

      _viewers = new List<ViewerComponent>();
      foreach (var layout in viewObj.layouts)
      {
        var prefab = new GameObject().AddComponent<ViewerComponent>();
        foreach (var viewer in layout.viewers)
        {
          var vc = Instantiate(prefab, transform);
          vc.name = name + viewer.Direction.TypeName();
          vc.Setup(viewer, colorStrip);

        }

      }


    }
  }
}