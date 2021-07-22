using System.Linq;
using UnityEngine;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{
  public static class ComponentMil
  {

    public static string TypeName(this object obj) => obj.GetType().ToString().Split('.').Last();

    public static TShell Build<TShell>(ViewObj obj, bool importIfValid = true) where TShell : ViewObjBehaviour
    {
      var shell = (TShell) new GameObject().AddComponent(typeof(TShell));

      if (importIfValid)
        shell.TryImport(obj);

      return shell;
    }

  }
}