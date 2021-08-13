using System;
using UnityEngine;

namespace ConnectorUnity.GUI
{
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