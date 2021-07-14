using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

namespace ScaleComp.Scripts
{

  [ExecuteInEditMode]
  public class SetToMap : MonoBehaviour
  {
    [SerializeField] private AbstractMap map;
    [SerializeField] private GameObject comp;

    [SerializeField] private ScaleCompPolyline compsPolyline;

    [SerializeField] private bool lockComp;
    [SerializeField] private bool eventSet;
    [SerializeField] private Vector2d cachedPos;

    private void Start()
    {
      Connect();
    }

    private void Update()
    {
      if (map == null)
      {
        eventSet = false;
        return;
      }

      if (!eventSet)
        Connect();

      if (lockComp)
      {
        cachedPos = map.WorldToGeoPosition(comp.transform.localPosition);
        Debug.Log($"Caching Comp Pos {cachedPos}");
        lockComp = false;
      }

      Debug.Log("root local scale " + map.Root.localScale);
      Debug.Log("map world scale " + map.WorldRelativeScale);

    }

    private void LockComp()
    {

      cachedPos = map.WorldToGeoPosition(comp.transform.localPosition);

    }
    
    private void Connect()
    {
      if (map == null) return;

      map.OnUpdated += SetCompScale;
      eventSet = true;
    }

    // TODO : Get Relative scale of object 
    // TODO : Lock in Position of Map 
    // TODO : Lock in relative scale 

    private void SetCompScale()
    {
      if (map == null || comp == null) return;

      // comp.transform.localScale = ;
      comp.transform.localPosition = map.GeoToWorldPosition(cachedPos);
      // comp.transform.localScale = Vector3.one * Mathf.Pow(2, map.Zoom);
    }
  }
}