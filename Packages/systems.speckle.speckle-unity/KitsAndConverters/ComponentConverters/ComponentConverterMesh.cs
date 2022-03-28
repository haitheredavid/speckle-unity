using System;
using System.Collections.Generic;
using System.Linq;
using Objects.Utils;
using Speckle.ConnectorUnity;
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
      // convert the mesh data
      MeshDataToNative(new[] { speckleMesh }, out var mesh, out var materials);

      var comp = New<MeshFilter>();
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

    /// <summary>
    ///
    /// </summary>
    /// <param name="meshes">meshes to be converted as SubMeshes</param>
    /// <param name="nativeMesh">The converted native mesh</param>
    /// <param name="nativeMaterials">The converted materials (one per converted sub-mesh)</param>
    private void MeshDataToNative(IReadOnlyCollection<Mesh> meshes, out UnityEngine.Mesh nativeMesh, out Material[] nativeMaterials)
    {
      var verts = new List<Vector3>();

      var uvs = new List<Vector2>();
      var vertexColors = new List<Color>();

      var materials = new List<Material>(meshes.Count);
      var subMeshes = new List<List<int>>(meshes.Count);

      foreach (Mesh m in meshes)
      {
        if (m.vertices.Count == 0 || m.faces.Count == 0) continue;

        List<int> tris = new List<int>();
        SubmeshToNative(m, verts, tris, uvs, vertexColors, materials);
        subMeshes.Add(tris);
      }
      nativeMaterials = materials.ToArray();

      nativeMesh = new UnityEngine.Mesh
      {
        subMeshCount = subMeshes.Count
      };

      nativeMesh.SetVertices(verts);
      nativeMesh.SetUVs(0, uvs);
      nativeMesh.SetColors(vertexColors);


      int j = 0;
      foreach (var subMeshTriangles in subMeshes)
      {
        nativeMesh.SetTriangles(subMeshTriangles, j);
        j++;
      }

      if (nativeMesh.vertices.Length >= UInt16.MaxValue)
        nativeMesh.indexFormat = IndexFormat.UInt32;

      nativeMesh.Optimize();
      nativeMesh.RecalculateBounds();
      nativeMesh.RecalculateNormals();
      nativeMesh.RecalculateTangents();
    }

    private void SubmeshToNative(Mesh speckleMesh, List<Vector3> verts, List<int> tris, List<Vector2> texCoords, List<Color> vertexColors, List<Material> materials)
    {
      speckleMesh.AlignVerticesWithTexCoordsByIndex();
      speckleMesh.TriangulateMesh();

      int indexOffset = verts.Count;

      // Convert Vertices
      verts.AddRange(ArrayToPoints(speckleMesh.vertices, speckleMesh.units));

      // Convert texture coordinates
      bool hasValidUVs = speckleMesh.TextureCoordinatesCount == speckleMesh.VerticesCount;
      if (speckleMesh.textureCoordinates.Count > 0 && !hasValidUVs)
        Debug.LogWarning(
          $"Expected number of UV coordinates to equal vertices. Got {speckleMesh.TextureCoordinatesCount} expected {speckleMesh.VerticesCount}. \nID = {speckleMesh.id}");

      if (hasValidUVs)
      {
        texCoords.Capacity += speckleMesh.TextureCoordinatesCount;
        for (int j = 0; j < speckleMesh.TextureCoordinatesCount; j++)
        {
          var (u, v) = speckleMesh.GetTextureCoordinate(j);
          texCoords.Add(new Vector2((float)u, (float)v));
        }
      }
      else if (speckleMesh.bbox != null)
      {
        //Attempt to generate some crude UV coordinates using bbox //TODO this will be broken for submeshes
        texCoords.AddRange(GenerateUV(verts, (float)speckleMesh.bbox.xSize.Length, (float)speckleMesh.bbox.ySize.Length));
      }

      // Convert vertex colors
      if (speckleMesh.colors != null)
      {
        if (speckleMesh.colors.Count == speckleMesh.VerticesCount)
        {
          vertexColors.AddRange(speckleMesh.colors.Select(c => c.ToUnityColor()));
        }
        else if (speckleMesh.colors.Count != 0)
        {
          //TODO what if only some submeshes have colors?
          Debug.LogWarning(
            $"{typeof(Mesh)} {speckleMesh.id} has invalid number of vertex {nameof(Mesh.colors)}. Expected 0 or {speckleMesh.VerticesCount}, got {speckleMesh.colors.Count}");
        }
      }

      // Convert faces
      tris.Capacity += (int)(speckleMesh.faces.Count / 4f) * 3;

      for (int i = 0; i < speckleMesh.faces.Count; i += 4)
      {
        //We can safely assume all faces are triangles since we called TriangulateMesh
        tris.Add(speckleMesh.faces[i + 1] + indexOffset);
        tris.Add(speckleMesh.faces[i + 3] + indexOffset);
        tris.Add(speckleMesh.faces[i + 2] + indexOffset);
      }

      // Convert RenderMaterial
      // materials.Add(GetMaterial(speckleMesh["renderMaterial"] as RenderMaterial));
    }

  }
}