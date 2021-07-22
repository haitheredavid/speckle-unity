using UnityEngine;
using ViewTo.Structure;

namespace ViewTo.Connector.Unity {


  public static class GoExtension {

    public static TComp GetOrSet<TComp>( this GameObject go ) where TComp : MonoBehaviour
      {
        TComp comp = go.GetComponent<TComp>( );
        if ( comp == null )
          comp = go.AddComponent<TComp>( );

        return comp;
      }
  }


  public class ViewerComponent : MonoBehaviour {

    // [SerializeField] private TYPE _type;
    [SerializeField] private ViewCamera cam;

    public void Setup( Viewer viewer, Texture2D colorStrip )
      {
        Align( viewer.Direction );
        CreateCam( );
        // CreateData( pointCount );
        // CreateProcessor( );
      }

    private void CreateCam( )
      {
        cam = gameObject.GetOrSet<ViewCamera>( );
        cam.CamDistance = 100000;
        cam.transform.SetPositionAndRotation( transform.position, transform.rotation );
        cam.transform.SetParent( transform );
      }

    private void Align( ViewerDirection dir )
      {
        Vector3 camDirection = dir switch {
          ViewerDirection.Front => new Vector3( 0, 0, 0 ),
          ViewerDirection.Left => new Vector3( 0, 90, 0 ),
          ViewerDirection.Back => new Vector3( 0, 180, 0 ),
          ViewerDirection.Right => new Vector3( 0, -90, 0 ),
          ViewerDirection.Up => new Vector3( 90, 0, 0 ),
          ViewerDirection.Down => new Vector3( -90, 0, 0 ),
          _ => new Vector3( 0, 0, 0 )
        };
        transform.localRotation = Quaternion.Euler( camDirection );
      }

  }
}