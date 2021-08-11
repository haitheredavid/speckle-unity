using UnityEngine;

namespace ViewTo.Connector.Unity
{
  public enum ImportStyle
  {
    Editor,
    Speckle2
  }

  [ExecuteAlways]
  public class ViewToController : MonoBehaviour
  {

    [SerializeField] private ImportStyle importStyle = ImportStyle.Editor;
    [SerializeField] private Material analysisMaterial;

    public static ViewToController Instance { get; set; }

    private void Awake() => Instance = this;
    
    
    
  }
}