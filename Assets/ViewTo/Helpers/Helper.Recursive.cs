using UnityEngine;

namespace ViewTo.Connector.Unity {
  public static class RecursiveHelper {

    public static void SetMeshVisibilityRecursive( this GameObject obj , bool status ){
        var mr = obj.GetComponent<MeshRenderer>( );
        if ( mr != null )
          mr.enabled = status;

        foreach ( Transform child in obj.transform ) {
          child.gameObject.SetMeshVisibilityRecursive( status );
        }
      }

    public static void SetLayerRecursively(this GameObject obj , int layer) {
        obj.layer = layer;

        foreach (Transform child in obj.transform) {
          child.gameObject.SetLayerRecursively(layer);
        }
      }

  }
}