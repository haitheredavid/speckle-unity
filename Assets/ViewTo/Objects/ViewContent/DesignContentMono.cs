using UnityEngine;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{
  public class DesignContentMono : ViewContentMono<DesignContent>
  {

    [SerializeField] private string viewName;

    protected override void ImportValidObj()
    {
      base.ImportValidObj();
      viewName = viewObj.viewName;

    }

    public bool DisplayMeshes
    {
      set
      {
        Debug.Log($"Setting {viewName} visibility to {value}");
        gameObject.SetMeshVisibilityRecursive(value);
      }
    }

  }
}