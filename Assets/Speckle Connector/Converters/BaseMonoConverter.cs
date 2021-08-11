using System.Linq;
using Speckle.Core.Kits;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  public class BaseMonoConverter<TKit> where TKit : ISpeckleKit
  {

    public BaseMonoConverter()
    {
      Kit = LoadKit();
    }

    public string app { get; set; } = Applications.Script;

    protected TKit Kit { get; set; }
    
    private TKit LoadKit()
    {
      foreach (var k in KitManager.Kits.ToList().Where(k => k.GetType() == typeof(TKit)))
      {
        if (k is TKit casted)
        {
          Debug.Log($"Found {casted.Name} with {casted.Types.Count()} types");
          return casted;
        }
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