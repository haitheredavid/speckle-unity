using System;
using UnityEngine.UIElements;

namespace Speckle.ConnectorUnity.GUI
{
  public static class ElementHelper
  {

    public static int DropDownChange(this DropdownField field, ChangeEvent<string> evt, Action<int> notify = null)
    {
      var index = -1;
      for (int i = 0; i < field.choices.Count; i++)
      {
        if (field.choices[i].Equals(evt.newValue))
        {
          index = i;
          break;
        }
      }

      if (index >= 0)
        notify?.Invoke(index);

      return index;
    }
  }
}