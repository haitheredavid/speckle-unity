using UnityEngine;
using ViewTo.Objects;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{

  public abstract class ViewObjBehaviour : MonoBehaviour, IValidator
  {
    public abstract void TryImport(ViewObj obj);
    public abstract bool isValid { get; }
  }

  public abstract class ViewObjBehaviour<TObj> : ViewObjBehaviour where TObj : ViewObj, new()
  {

    private TObj _internalObj;

    protected TObj viewObj
    {
      get => _internalObj ??= new TObj();
      set => _internalObj = value;
    }

    public override bool isValid
    {
      get
      {
        return !(viewObj is IValidator va) || va.isValid;
      }
    }
    
    // public abstract TObj CopyObj();

    public override void TryImport(ViewObj obj)
    {
      switch (obj)
      {
        case null:
          return;
        case TObj casted:
          // if (casted is IValidator va && !va.isValid)
          //   return;
    
          viewObj = casted;
          ImportValidObj();
          break;
      }
    }
    
    protected virtual void ImportValidObj()
    { }
  }

}