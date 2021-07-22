using HaiThere;
using UnityEngine;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{

  public abstract class ViewObjBehaviour : MonoBehaviour
  {
    public abstract void TryImport(ViewObj obj);
  }

  public abstract class ViewObjBehaviour<TObj> : ViewObjBehaviour where TObj : ViewObj
  {

    public TObj viewObj { get; protected set; }

    protected abstract void ImportValidObj();
    
    public override void TryImport(ViewObj obj)
    {
      switch (obj)
      {
        case null:
          return;
        case TObj casted when obj is IValidator va:
        {
          if (va.isValid)
            ImportValidObj();
          break;
        }
        case TObj casted:
          ImportValidObj();
          break;
      }

    }
  }

}