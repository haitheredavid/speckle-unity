using System;
using UnityEngine;

namespace ConnectorUnity
{

  [AttributeUsage(AttributeTargets.Field)]
  public class ReadOnlyAttribute : PropertyAttribute
  { }

  [AttributeUsage(AttributeTargets.Method)]
  public class ButtonAttribute : PropertyAttribute
  {
    public string Text { get; }

    public ButtonAttribute(string value = null)
    {
      Text = value;
    }
  }
}