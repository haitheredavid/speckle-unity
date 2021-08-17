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

    protected override void ImportValidObj()
    {
      ContentMask = MaskByType();
      SetGeo();
      SetContentData(viewObj);
    }

    /// <summary>
    /// references the objects converted to the view content list and imports them 
    /// </summary>
    protected virtual void SetGeo()
    {
      foreach (var obj in viewObj.objects)
      {
        if (obj is GameObject go)
          go.transform.SetParent(transform);
        else if (obj is Mesh mesh)
        {
          var mf = new GameObject().AddComponent<MeshFilter>();
          mf.gameObject.AddComponent<MeshRenderer>();

          if (Application.isPlaying)
            mf.mesh = mesh;
          else
            mf.sharedMesh = mesh;

          mf.transform.SetParent(transform);
        }
      }

    }

    protected abstract void SetContentData(ViewContent t);

    private int MaskByType() => viewObj switch
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

    // public override ViewContent CopyObj()
    // {
    //   return new TContent {viewColor = ViewColor};
    // }

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