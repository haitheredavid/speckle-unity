using UnityEditor;
using UnityEngine;

namespace ConnectorUnity
{
  public static class SpeckleGUIStyle
  {
    public static void HorizontalLine(Rect rect, float height, Color color)
    {
      rect.height = height;
      EditorGUI.DrawRect(rect, color);
    }

    public static GUIStyle LabelStyle()
    {
      return new GUIStyle(EditorStyles.centeredGreyMiniLabel)
      {
        fontStyle = FontStyle.Normal,
        alignment = TextAnchor.UpperCenter
      };
    }

    public static GUIStyle ButtonStyle()
    {
      return new GUIStyle(EditorStyles.miniButton)
      {
        fontStyle = FontStyle.Bold,
        alignment = TextAnchor.MiddleCenter,
        richText = true
      };
    }

  }
}