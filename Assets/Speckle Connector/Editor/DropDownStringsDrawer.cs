using System;
using System.Reflection;
using ConnectorUnity.GUI;
using UnityEditor;
using UnityEngine;

namespace ConnectorUnity
{

  [CustomPropertyDrawer(typeof(DropDownStringAttribute))]
  public class DropDownStringsDrawer : PropertyDrawer
  {
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

      var listAttribute = attribute as DropDownStringAttribute;
      var methodValuesInfo = GetValues(property, listAttribute.Name);
      if (methodValuesInfo is Array valuesList)
      {
        string[] displayOptions = new string[valuesList.Length];

        for (int i = 0; i < valuesList.Length; i++)
        {
          object value = valuesList.GetValue(i);
          displayOptions[i] = value == null ? "<null>" : value.ToString();
        }
        property.intValue = EditorGUI.Popup(position, property.displayName, property.intValue, displayOptions, EditorStyles.popup);

      }

    }
    private object GetValues(SerializedProperty property, string valuesName)
    {
      object target = PropertyUtility.GetTargetObjectWithProperty(property);

      FieldInfo valuesFieldInfo = ReflectionUtility.GetField(target, valuesName);
      if (valuesFieldInfo != null)
      {
        return valuesFieldInfo.GetValue(target);
      }

      PropertyInfo valuesPropertyInfo = ReflectionUtility.GetProperty(target, valuesName);
      if (valuesPropertyInfo != null)
      {
        return valuesPropertyInfo.GetValue(target);
      }

      MethodInfo methodValuesInfo = ReflectionUtility.GetMethod(target, valuesName);
      if (methodValuesInfo != null && methodValuesInfo.ReturnType != typeof(void) && methodValuesInfo.GetParameters().Length == 0)
      {
        return methodValuesInfo.Invoke(target, null);
      }

      return null;
    }
  }
}