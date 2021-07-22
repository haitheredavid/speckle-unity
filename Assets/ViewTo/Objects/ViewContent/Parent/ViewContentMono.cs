using UnityEngine;
using ViewTo.Objects;
using ViewTo.Structure;

namespace ViewTo.Connector.Unity
{

  public interface IViewContentMono
  {
    ViewColor ViewColor { get; }
    void PrepContentMeshes(Material material);
    void DestroySelf();
  }

  public abstract class ViewContentMono<TContent> : ViewObjBehaviour<TContent>, IViewContentMono
    where TContent : ViewContent
  {

    [SerializeField] protected bool useEditorColor = false;
    [SerializeField] private Color32 viewColor = Color.magenta;
    [ReadOnly] [SerializeField] private int viewColorID;
    [ReadOnly] [SerializeField] private int layerMask;

    public ViewColor ViewColor
    {
      get => viewColor.ToNative(viewColorID);
      protected set
      {
        viewColor = value.ToUnity();
        viewColorID = value.Id;
      }
    }
    protected override void ImportValidObj()
    {
      ViewColor = viewObj.viewColor;
    }

    public void PrepContentMeshes(Material material)
    {
      var mat = Instantiate(new Material(material) {color = viewColor});

      foreach (Transform child in transform)
      {
        var co = child.gameObject.AddComponent<ContentObject>();
        co.CombineMeshes(mat);
      }
      layerMask = viewObj.MaskByType();
      gameObject.SetLayerRecursively(layerMask);
    }
    
    
    
    public void DestroySelf()
    {
      Destroy(gameObject);
    }
  }

}