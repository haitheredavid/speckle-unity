using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Mesh = Objects.Geometry.Mesh;

namespace Objects.Converter.Unity
{
  [CreateAssetMenu(fileName = "MeshComponentConverter", menuName = "Speckle/Mesh Converter Component")]
  public class ComponentConverterMesh : ComponentConverter<Mesh>
  {
    public bool addMeshCollider = false;
    public bool addRender = true;
    public bool recenterTransform = true;

    protected static List<int> FaceToTriangles(List<int> faces)
    {
      //convert speckleMesh.faces into triangle array           
      var tris = new List<int>();
      var i = 0;
      // TODO: Check if this is causing issues with normals for mesh 
      while (i < faces.Count)
        if (faces[i] == 0)
        {
          //Triangles
          tris.Add(faces[i + 1]);
          tris.Add(faces[i + 3]);
          tris.Add(faces[i + 2]);

          i += 4;
        }
        else
        {
          //Quads to triangles
          tris.Add(faces[i + 1]);
          tris.Add(faces[i + 3]);
          tris.Add(faces[i + 2]);

          tris.Add(faces[i + 1]);
          tris.Add(faces[i + 4]);
          tris.Add(faces[i + 3]);

          i += 5;
        }

      return tris;
    }

    /// <summary>
    /// Converts a SpeckleMesh with a material to a Mesh Filter
    /// </summary>
    /// <param name="speckleMesh"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    public virtual Component ToComponent(Mesh speckleMesh, Material material)
    {
      var mesh = base.ToComponent(speckleMesh);

      if (mesh != null)
      {
        var render = mesh.gameObject.GetComponent<MeshRenderer>();
        if (IsRuntime)
          render.sharedMaterial = material;
        else
          render.material = material;
      }

      return mesh;
    }

    /// <summary>
    /// Converts a SpeckleMesh to a Mesh Filter 
    /// </summary>
    /// <param name="speckleMesh"></param>
    /// <returns></returns>
    protected override Component Process(Mesh speckleMesh)
    {
      if (speckleMesh.vertices.Count == 0 || speckleMesh.faces.Count == 0)
      {
        Debug.LogWarning("Trying to convert speckle mesh without proper data");
        return null;
      }

      var mesh = new UnityEngine.Mesh { name = speckleMesh.speckle_type };

      var verts = ArrayToPoints(speckleMesh.vertices, speckleMesh.units);

      if (verts.Length >= 65535)
        mesh.indexFormat = IndexFormat.UInt32;

      // center transform pivot according to the bounds of the model
      var meshBounds = new Bounds { center = verts[0] };

      foreach (var vert in verts)
        meshBounds.Encapsulate(vert);

      // setup object for data
      var comp = New<MeshFilter>();
      if (recenterTransform)
        comp.transform.position = meshBounds.center;

      // offset mesh vertices
      for (var l = 0; l < verts.Length; l++)
        verts[l] -= meshBounds.center;

      mesh.SetVertices(verts);
      mesh.SetTriangles(FaceToTriangles(speckleMesh.faces), 0);

      if (speckleMesh.bbox != null)
      {
        var uv = GenerateUV(verts, (float)speckleMesh.bbox.xSize.Length, (float)speckleMesh.bbox.ySize.Length).ToList();
        mesh.SetUVs(0, uv);
      }

      // BUG: causing some funky issues with meshes
      // mesh.RecalculateNormals( );
      mesh.Optimize();

      // Setting mesh to filter once all mesh modifying is done
      if (IsRuntime)
        comp.mesh = mesh;
      else
        comp.sharedMesh = mesh;

      if (addMeshCollider)
        comp.gameObject.AddComponent<MeshCollider>().sharedMesh = IsRuntime ? comp.mesh : comp.sharedMesh;

      if (storeProps)
        comp.gameObject.AddProps(speckleMesh);

      if (addRender)
        comp.gameObject.AddComponent<MeshRenderer>();

      return comp;
    }
  }
}