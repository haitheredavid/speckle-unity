using System;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{
  public static partial class ViewConverter
  {

    public static ContentBundleMono ToUnity(this ContentBundle obj, bool importIfValid = true) => ToUnity<ContentBundleMono>(obj, importIfValid);
    
    public static TargetContentMono ToUnity(this TargetContent obj, bool importIfValid = true) => ToUnity<TargetContentMono>(obj, importIfValid);

    public static BlockerContentMono ToUnity(this BlockerContent obj, bool importIfValid = true) => ToUnity<BlockerContentMono>(obj, importIfValid);

    public static DesignContentMono ToUnity(this DesignContent obj, bool importIfValid = true) => ToUnity<DesignContentMono>(obj, importIfValid);
    
    public static ViewContentMono ToUnity(this ViewContent obj)
    {
      return obj switch
      {
        TargetContent o => o.ToUnity<TargetContentMono>(),
        DesignContent o => o.ToUnity<DesignContentMono>(),
        BlockerContent o => o.ToUnity<BlockerContentMono>(),
        _ => throw new ArgumentOutOfRangeException(nameof(obj), obj, null)
      };
    }

  }
}