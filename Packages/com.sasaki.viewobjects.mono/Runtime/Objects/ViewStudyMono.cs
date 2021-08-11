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

    public List<ViewObj> ViewObjs
    {
      get => viewObj.objs.Valid() ? viewObj.objs : new List<ViewObj>();
      set => viewObj.objs = value;
    }



    public void SetHackyContent(List<object> content, List<GameObject> meshes)
    {
      foreach (var obj in loadedObjs)
        if (obj is ContentBundleMono mono)
          mono.SetHackyContent(content, meshes);

    }
  

    public override ViewStudy CopyObj()
    {
      return new ViewStudy
        {objs = viewObj.objs, viewName = viewObj.viewName};
    }
    protected override void ImportValidObj()
    {
      viewName = viewObj.viewName;
      gameObject.name = viewName.Valid() ? viewName : viewObj.TypeName();

      loadedObjs = new List<ViewObjBehaviour>();
      // load all objects
      foreach (var obj in ViewObjs)
      {
        Debug.Log($"Loading new object: {obj.TypeName()} ");
        var item = obj.ConvertToViewMono();
        item.transform.SetParent(transform);
        loadedObjs.Add(item);
      }

    }
  }
}