using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Api;
using Speckle.Core.Credentials;
using ViewTo.Objects.Structure;

namespace Speckle_Connector
{
  public static class StreamManagerHelper
  {
    public static string[] Format(this IEnumerable<Account> items)
    {
      return items.Select(x => x.userInfo.email + " | " + x.serverInfo.name).ToArray();
    }

    public static string[] Format(this IEnumerable<Stream> items)
    {
      return items.Select(x => x.name + " | " + x.id).ToArray();
    }

    public static string[] Format(this IEnumerable<Branch> items)
    {
      return items.Select(x => x.name).ToArray();
    }
    public static string[] Format(this IEnumerable<Commit> items)
    {
      return items.Select(x => x.id ).ToArray();
    }
  }
}