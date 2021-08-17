using UnityEngine;
using ViewTo.Objects;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{
  public class DesignContentMono : ViewContentMono<DesignContent>, IViewName
  {
    [SerializeField] private string viewObjectName;

    public string viewName
    {
      get => viewObjectName;
      set => viewObjectName = value;
    }

    // public override ViewContent CopyObj()
    // {
    //   return new DesignContent
    //   {
    //     viewName = viewName, viewColor = ViewColor
    //   };
    // }
    protected override void SetValidContent(DesignContent content)
    {
      viewName = content.viewName;
      gameObject.name = viewName;
    }
  }
}