using UnityEngine;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{
  public static partial class ViewConverter
  {

    public static ViewColor ToView(this Color32 value, int index) => new ViewColor(value.r, value.g, value.b, value.a, index);

    public static Color32 ToUnity(this ViewColor value) => new Color32(value.R, value.G, value.B, value.A);
  }
}