using UnityEngine;
using ViewTo.Objects;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{

  public abstract class ViewContentMono : ViewObjBehaviour<ViewContent>
  {

    [SerializeField] private Color32 viewColor = Color.magenta;
    [ReadOnly] [SerializeField] private int viewColorID;
    [ReadOnly] [SerializeField] private int contentMask;

    public int ContentMask
    {
      get => contentMask;
      set => contentMask = value;
    }

    public ViewColor ViewColor
    {
      get => viewObj.viewColor;
      set
      {
        viewObj.viewColor = value;
        viewColor = value.ToUnity();
        viewColorID = value.Id;
      }
    }

    public override ViewContent CopyObj() => throw new System.NotImplementedException();

    protected override void ImportValidObj()
    {
      ViewColor = viewObj.viewColor;
      ContentMask = MaskByType(viewObj);
      SetContentData(viewObj);
    }

    protected abstract void SetContentData(ViewContent t);

    private static int MaskByType(ViewContent t) => t switch
    {
      DesignContent _ => 6,
      TargetContent _ => 7,
      BlockerContent _ => 8,
      _ => 0
    };

  }

  public abstract class ViewContentMono<TContent> : ViewContentMono
    where TContent : ViewContent, new()
  {

    public override ViewContent CopyObj()
    {
      return new TContent {viewColor = ViewColor};
    }

    protected virtual void SetValidContent(TContent content)
    {
      gameObject.name = content.TypeName();
    }

    protected override void SetContentData(ViewContent t)
    {
      if (t is TContent casted)
        SetValidContent(casted);
    }

  }

}