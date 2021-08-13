using System;
using UnityEngine;

namespace ConnectorUnity.GUI
{

  [AttributeUsage(AttributeTargets.Field)]
  public class DropDownStringAttribute : PropertyAttribute
  {
    public string Name { get; }

    public DropDownStringAttribute(string value)
    {
      Name = value;
    }
  }
}