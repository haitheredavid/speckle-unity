using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Kits;
using UnityEngine;

namespace ConnectorUnity
{
  public static class ConnectorHelper
  {
    public static TObject CheckList<TObject>(this List<TObject> list, int index)
    {
      if (list == null || index >= list.Count)
      {
        Debug.LogWarning($"{(list == null ? "null" : list.GetType().ToString().Split('.').Last())} is out of range for that index, defaulting to first index");
        return default;
      }
      return list[index];
    }

    public static string[] Format(this IEnumerable<ISpeckleKit> items)
    {
      return items != null ? items.Select(x => x.Name + " | " + x.Author).ToArray() : new[] {"empty"};
    }

    public static string[] Format(this IEnumerable<Account> items)
    {
      return items != null ? items.Select(x => x.userInfo.email + " | " + x.serverInfo.name).ToArray() : new[] {"empty"};
    }

    public static string[] Format(this IEnumerable<Stream> items)
    {
      return items != null ? items.Select(x => x.name + " | " + x.id).ToArray() : new[] {"empty"};
    }

    public static string[] Format(this IEnumerable<Branch> items)
    {
      return items != null ? items.Select(x => x.name).ToArray() : new[] {"empty"};
    }
    public static string[] Format(this IEnumerable<Commit> items)
    {
      return items != null ? items.Select(x => x.id).ToArray() : new[] {"empty"};
    }
  }
}