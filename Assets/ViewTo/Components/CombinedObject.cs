using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ViewTo.Connector.Unity {
  public class ContentObject : MonoBehaviour {

    private CombineInstance[ ] CombineAllMeshes( )
      {
        var meshFilters = GetComponentsInChildren<MeshFilter>( );
        Debug.Log( $"Compiling {meshFilters.Length} Meshes" );

        var combine = new List<CombineInstance>( );

        foreach ( var t in meshFilters ) {
        
          var temp = Instantiate( t.sharedMesh );
          
          var c = new CombineInstance {
            mesh = temp,
            transform = t.transform.localToWorldMatrix
          };
          
          combine.Add( c );
        }
        return combine.ToArray( );
      }

    private GameObject CombineToObject( )
      {
        var combine = CombineAllMeshes( );

        if ( combine == null ) {
          Debug.Log( $"Meshes for {gameObject.name} did not combine properly" );
          return null;
        }
        var go = new GameObject( "Mesh" );
        go.transform.SetParent( transform );

        var filter = go.AddComponent<MeshFilter>( );
        var mesh = filter.sharedMesh = new Mesh( );

        // NOTE changing this allows for larger vertex count
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.name = "combined";

        mesh.CombineMeshes( combine );

        foreach ( var t in combine )
          Destroy( t.mesh );

        return go;
      }


    public void CombineMeshes( Material material )
      {
        var go = CombineToObject( );

        if ( go == null ) return;

        var rend = go.AddComponent<MeshRenderer>( );

        rend.sharedMaterial = material;
        rend.shadowCastingMode = ShadowCastingMode.Off;
        rend.allowOcclusionWhenDynamic = rend.receiveShadows = false;

        foreach ( Transform child in transform )
          Destroy( child.gameObject );
      }

  }

}