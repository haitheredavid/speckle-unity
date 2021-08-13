using System.Linq;
using Speckle.Core.Kits;
using UnityEngine;

namespace ConnectorUnity.Converters
{

  public class MonoConverter<TKit> where TKit : ISpeckleKit
  {

    public MonoConverter()
    {
      Kit = LoadKit();
    }
    
    protected TKit Kit { get; set; }

    private TKit LoadKit()
    {
      var k = KitManager.GetKit(Kit.GetType().Assembly.FullName);
      if (k is TKit casted)
      {
        Debug.Log($"Found {casted.Name} with {casted.Types.Count()} types");
        return casted;
      }
      return default;
    }

    protected TConverter LoadConverter<TConverter>(string app) where TConverter : ISpeckleConverter
    {
      Kit ??= LoadKit();
      if (Kit != null)
      {
        var res = Kit.LoadConverter(app);
        if (res is TConverter c)
          return c;
      }
      return default;
    }

  }
}