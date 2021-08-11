using System.Collections.Generic;
using UnityEngine;
using ViewTo.Objects;
using ViewTo.Objects.Elements;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{

  public class ViewerBundleMono : ViewObjBehaviour<ViewerBundle>
  {

    [SerializeField] private bool global;
    [SerializeField] private Texture2D colorStrip;
    [SerializeField] private List<string> clouds;
    [SerializeField] private List<Color32> colors;

    public bool hasLinks
    {
      get => linkedShell != null && linkedShell.Count != 0;
    }

    public List<MetaShell> linkedShell { get; private set; }

    public int viewerCount { get; private set; }
    public List<ViewerMono> viewers { get; set; }

    public List<ViewerLayout> layouts
    {
      get => viewObj?.layouts;
    }

    public bool IsGlobal
    {
      get => global;
      set => global = value;
    }

    public override ViewerBundle CopyObj()
    {
      return hasLinks ?
        new ViewerBundleLinked() {layouts = viewObj.layouts, linkedClouds = linkedShell} :
        new ViewerBundle() {layouts = layouts};
    }

    protected override void ImportValidObj()
    {
      gameObject.name = viewObj.TypeName();
      viewerCount = 0;
      foreach (var l in layouts) viewerCount += l.viewers.Count;

      if (viewObj is ViewerBundleLinked linked && linked.linkedClouds.Valid())
        linkedShell = linked.linkedClouds;


    }
  }
}