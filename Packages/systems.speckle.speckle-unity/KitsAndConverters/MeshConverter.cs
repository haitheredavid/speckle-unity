using System.Collections.Generic;
using System.Linq;
using Speckle.Core.Models;
using UnityEngine;
using UnityEngine.Rendering;

namespace Objects.Converter.Unity
{

  public class MeshConverter : ConverterComponent
  {

    public virtual Mesh ToComponent(Base @base)
    {
      if (@base is Geometry.Mesh mesh)
        return ToComponent(mesh);

      if (@base["displayMesh"] is Geometry.Mesh displayMesh) return ToComponent(displayMesh);

      // TODO: catch this properly 
      return null;
    }

    public virtual Mesh ToComponent(Geometry.Mesh speckleMesh)
    {
      if (speckleMesh.vertices.Count == 0 || speckleMesh.faces.Count == 0)
      {
        Debug.LogWarning("Trying to convert speckle mesh without proper data");
        return null;
      }


      var verts = ArrayToPoints(speckleMesh.vertices, speckleMesh.units);

      //convert speckleMesh.faces into triangle array           
      var tris = new List<int>();
      var i = 0;
      // TODO: Check if this is causing issues with normals for mesh 
      while (i < speckleMesh.faces.Count)
        if (speckleMesh.faces[i] == 0)
        {
          //Triangles
          tris.Add(speckleMesh.faces[i + 1]);
          tris.Add(speckleMesh.faces[i + 3]);
          tris.Add(speckleMesh.faces[i + 2]);

          i += 4;
        }
        else
        {
          //Quads to triangles
          tris.Add(speckleMesh.faces[i + 1]);
          tris.Add(speckleMesh.faces[i + 3]);
          tris.Add(speckleMesh.faces[i + 2]);

          tris.Add(speckleMesh.faces[i + 1]);
          tris.Add(speckleMesh.faces[i + 4]);
          tris.Add(speckleMesh.faces[i + 3]);

          i += 5;
        }


      var mesh = new Mesh { name = speckleMesh.speckle_type };

      if (verts.Length >= 65535)
        mesh.indexFormat = IndexFormat.UInt32;


      // center transform pivot according to the bounds of the model
      var meshBounds = new Bounds { center = verts[0] };

      foreach (var vert in verts)
        meshBounds.Encapsulate(vert);

      // go.transform.position = meshBounds.center;

      // offset mesh vertices
      for (var l = 0; l < verts.Length; l++) verts[l] -= meshBounds.center;

      mesh.SetVertices(verts);
      mesh.SetTriangles(tris, 0);

      if (speckleMesh.bbox != null)
      {
        var uv = GenerateUV(verts, (float)speckleMesh.bbox.xSize.Length, (float)speckleMesh.bbox.ySize.Length).ToList();
        mesh.SetUVs(0, uv);
      }

      // BUG: causing some funky issues with meshes
      // mesh.RecalculateNormals( );
      mesh.Optimize();
      return mesh;

    }
  }
}