using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mesh = Objects.Geometry.Mesh;

namespace ConnectorUnity
{
  public partial class ConverterUnity
  {
    public UnityEngine.Mesh MeshToUnity(Mesh speckleMesh)
    {
      if (speckleMesh.vertices.Count == 0 || speckleMesh.faces.Count == 0)
        return null;

      var verts = ArrayToPoints(speckleMesh.vertices, speckleMesh.units);
      //convert speckleMesh.faces into triangle array           
      List<int> tris = new List<int>();
      int i = 0;
      // TODO: Check if this is causing issues with normals for mesh 
      while (i < speckleMesh.faces.Count)
      {
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
      }


      var mesh = new UnityEngine.Mesh
      {
        name = speckleMesh.speckle_type +":"+ speckleMesh.id
      };

      if (verts.Length >= 65535)
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

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