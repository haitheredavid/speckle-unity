using System.Reflection;
using ConnectorUnity.GUI;
using UnityEditor;
using UnityEngine;

namespace ConnectorUnity
{
  [CustomPropertyDrawer(typeof(ManagerData))]
  public class SpeckleManagerInputDrawer : PropertyDrawer
  {
    private const int SIZE = 20;
    private const int TOTAL = 5;

    private readonly string[] PropNames = {"account", "stream", "branch", "commit", "kit"};

    private Rect CreateRect(Rect r, float offset, float height)
    {
      return new Rect(r.x, r.y + offset, r.width, height);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return TOTAL * base.GetPropertyHeight(property, label) + 10;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      position.width -= 5;
      EditorGUI.BeginProperty(position, label, property);
      // var contentPos = EditorGUI.PrefixLabel(position, label);
      Rect bounds = default;
      foreach (var props in PropNames)
      {
        bounds = bounds == default ?
          CreateRect(position, 0, SIZE) : CreateRect(bounds, SIZE, SIZE);
        EditorGUI.PropertyField(bounds, property.FindPropertyRelative(props), GUIContent.none);
      }
      EditorGUI.EndProperty();

    }

  }

}