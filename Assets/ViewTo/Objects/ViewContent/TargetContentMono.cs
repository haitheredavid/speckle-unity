using System.Collections.Generic;
using UnityEngine;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{

  public class TargetContentMono : ViewContentMono<TargetContent>
  {
    [SerializeField] private string viewName;
    [SerializeField] private bool isolate;

    public List<ViewerBundle> bundles { get; set; }
    
    protected override void ImportValidObj()
    {
      base.ImportValidObj();
      viewName = viewObj.viewName;
      bundles = viewObj.bundles;
      isolate = viewObj.isolate;

    }

  }
}