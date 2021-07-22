using System;
using UnityEngine;
using ViewTo.Objects;

namespace ViewTo.Connector.Unity
{
  public class ViewStudyMono : ViewObjBehaviour<ViewStudy>
  {

    [SerializeField] private string viewName;
    
    [SerializeField] private ViewCloudMono cloudMono;
    [SerializeField] private ContentBundleMono content;
    [SerializeField] private Rig rig;

    [SerializeField] private bool saveOverResults = true;

    public event Action<bool> SetupEvent;
    public bool IsReady { get; private set; }

    private void Awake()
    {
      SetComponentElements();
    }
    protected override void ImportValidObj()
    {
      viewName = viewObj.viewName;
      // TODO unwrap study
      
    }

    private void SetComponentElements()
    {
      SetupEvent += b =>
      {
        {
          Debug.Log($"Setup Update: {viewName} is ready? {b} ");
          IsReady = b;
        }
      };
    }

  }
}