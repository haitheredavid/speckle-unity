using UnityEngine;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{
  public static partial class ViewConverter
  {
    public static RigMono ToUnity(this RigObj obj, bool importIfValid = true) => obj.ToUnity<RigMono>(importIfValid);

    public static ViewerBundleMono ToUnity(this ViewerBundle obj, bool importIfValid = true) => ToUnity<ViewerBundleMono>(obj, importIfValid);

    public static ViewStudyMono ToUnity(this ViewStudy obj, bool importIfValid = true) => ToUnity<ViewStudyMono>(obj, importIfValid);

    public static ViewCloudMono ToUnity(this ViewCloud obj, bool importIfValid = true) => ToUnity<ViewCloudMono>(obj, importIfValid);

    public static TShell ToUnity<TShell>(this ViewObj obj, bool importIfValid = true) where TShell : ViewObjBehaviour
    {
      var shell = (TShell)new GameObject().AddComponent(typeof(TShell));

      if (importIfValid)
        shell.TryImport(obj);

      return shell;
    }
    public static TShell SetMono<TShell>(this GameObject go, ViewObj obj) where TShell : ViewObjBehaviour
    {
      var shell = (TShell)go.AddComponent(typeof(TShell));
      shell.TryImport(obj);
      return shell;
    }

    public static ViewObjBehaviour ConvertToViewMono(this object obj)
    {
      var go = new GameObject("Empty");
      return obj switch
      {
        ViewStudy o => go.SetMono<ViewStudyMono>(o),
        ViewCloud o => go.SetMono<ViewCloudMono>(o),
        RigObj o => go.SetMono<RigMono>(o),
        ViewerBundle o => go.SetMono<ViewerBundleMono>(o),
        ContentBundle o => go.SetMono<ContentBundleMono>(o),
        ViewContent o => go.SetMono<ViewContentMono>(o),
        _ => null
      };
    }

  }
}