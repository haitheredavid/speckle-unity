using System;
using UnityEngine;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{
  public static partial class ViewConverter
  {

    public static ViewCloudMono SetMono(this ViewCloud obj) => obj.SetMono<ViewCloudMono>();
    public static ResultCloudMono SetMono(this ResultCloud obj) => obj.SetMono<ResultCloudMono>();

    public static ViewStudyMono SetMono(this ViewStudy obj) => obj.SetMono<ViewStudyMono>();
    public static ContentBundleMono SetMono(this ContentBundle obj) => obj.SetMono<ContentBundleMono>();

    public static RigMono SetMono(this RigObj obj) => obj.SetMono<RigMono>();
    public static ViewerBundleMono SetMono(this ViewerBundle obj) => obj.SetMono<ViewerBundleMono>();

    public static TargetContentMono SetMono(this TargetContent obj) => obj.SetMono<TargetContentMono>();
    public static BlockerContentMono SetMono(this BlockerContent obj) => obj.SetMono<BlockerContentMono>();
    public static DesignContentMono SetMono(this DesignContent obj) => obj.SetMono<DesignContentMono>();

    public static TShell SetMono<TShell>(this ViewObj obj) where TShell : ViewObjBehaviour
    {
      return new GameObject().SetMono<TShell>(obj);
    }

    public static TShell SetMono<TShell>(this GameObject go, ViewObj obj) where TShell : ViewObjBehaviour
    {
      var shell = (TShell)go.AddComponent(typeof(TShell));
      shell.TryImport(obj);
      return shell;
    }

    public static ViewContentMono SetMono( this ViewContent obj)
    {
      return obj switch
      {
        TargetContent o => o.SetMono<TargetContentMono>(),
        DesignContent o => o.SetMono<DesignContentMono>(),
        BlockerContent o => o.SetMono<BlockerContentMono>(),
        _ => throw new ArgumentOutOfRangeException(nameof(obj), obj, null)
      };
    }
  }
}