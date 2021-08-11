using System.Collections.Generic;
using System.Linq;
using ViewTo.Objects.Elements;

namespace ViewTo.Connector.Unity
{
  public static partial class ViewConverter
  {

    public static List<CloudShell> ToUnity(this Dictionary<string, CloudPoint[]> obj) => obj.Select(i => new CloudShell
      {
        count = i.Value.Length,
        id = i.Key,
        points = i.Value.ToUnity(out var m),
        meta = m
      })
      .ToList();
  }
}