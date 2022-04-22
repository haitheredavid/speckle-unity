using System.Collections;

namespace Speckle.ConnectorUnity
{
  public static class RandomUtils
  {

    public static bool Valid(this IList list) => list.Valid(0);

    public static bool Valid(this IList list, int count) => list != null && count >= 0 && count < list.Count;

    public static bool Valid(this ICollection list) => list.Valid(0);

    public static bool Valid(this ICollection list, int count) => list != null && count >= 0 && count < list.Count;
  }
}