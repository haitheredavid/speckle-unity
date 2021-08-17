using System.Collections.Generic;
using UnityEngine;
using ViewTo.Objects;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{
  public class ViewStudyMono : ViewObjBehaviour<ViewStudy>
  {

    [SerializeField] private string viewName;
    [SerializeField] private List<ViewObjBehaviour> loadedObjs;

    public string ViewName
    {
      get => viewName;
      set
      {
        viewName = value;
        name = value;
      }
    }

    public List<ViewObjBehaviour> ViewObjs
    {
      get => loadedObjs;
      set => loadedObjs = value;
    }

    // public override ViewStudy CopyObj()
    // {
    //   return new ViewStudy
    //     {objs = viewObj.objs, viewName = viewObj.viewName};
    // }
    //
    // protected override void ImportValidObj()
    // {
    //   viewName = viewObj.viewName;
    //   gameObject.name = viewName.Valid() ? viewName : viewObj.TypeName();
    // }
  }
}