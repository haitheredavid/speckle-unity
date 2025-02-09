﻿using Objects.Geometry;
using Objects.Other;
using Objects.Utils;
using System.Collections.Generic;
using System.Linq;
using Speckle.ConnectorUnity.Core.ScriptableConverter.Components;
using UnityEngine;
using UnityEngine.Rendering;
using SMesh = Objects.Geometry.Mesh;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter
{


    public static partial class ConverterUtils
    {

        public static bool IsRuntime => Application.isPlaying;



        public static SMesh MeshToSpeckle(this IMeshableConverter converter, MeshFilter component)
        {
            var nativeMesh = IsRuntime ? component.mesh : component.sharedMesh;

            var nTriangles = nativeMesh.triangles;
            var sFaces = new List<int>(nTriangles.Length * 4);
            for (var i = 2; i < nTriangles.Length; i += 3)
            {
                sFaces.Add(0); //Triangle cardinality indicator

                sFaces.Add(nTriangles[i]);
                sFaces.Add(nTriangles[i - 1]);
                sFaces.Add(nTriangles[i - 2]);
            }

            var nVertices = nativeMesh.vertices;
            var sVertices = new List<double>(nVertices.Length * 3);

            foreach (var vertex in nVertices)
            {
                var p = component.gameObject.transform.TransformPoint(vertex);
                sVertices.Add(p.x);
                sVertices.Add(p.z); //z and y swapped
                sVertices.Add(p.y);
            }

            var nColors = nativeMesh.colors;
            var sColors = new List<int>(nColors.Length);
            sColors.AddRange(nColors.Select(c => c.ToIntColor()));

            var nTexCoords = nativeMesh.uv;
            var sTexCoords = new List<double>(nTexCoords.Length * 2);
            foreach (var uv in nTexCoords)
            {
                sTexCoords.Add(uv.x);
                sTexCoords.Add(uv.y);
            }

            // NOTE: this throws some exceptions with trying to set a method that isn't settable.
            // Looking at other converters it seems like the conversion code should be handling all the prop settings..

            //
            // // get the speckle data from the go here
            // // so that if the go comes from speckle, typed props will get overridden below
            // // TODO: Maybe handle a better way of overriding props? Or maybe this is just the typical logic for connectors 
            // if (convertProps)
            // {
            //   // Base behaviour is the standard unity mono type that stores the speckle props data
            //   var baseBehaviour = component.GetComponent(typeof(BaseBehaviour)) as BaseBehaviour;
            //   if (baseBehaviour != null && baseBehaviour.properties != null)
            //   {
            //     baseBehaviour.properties.AttachUnityProperties(mesh, excludedProps);
            //   }
            // }

            return new SMesh
            {
                vertices = sVertices,
                faces = sFaces,
                colors = sColors,
                textureCoordinates = sTexCoords,
                units = ModelUnits
            };
        }

        public static void MeshToNative(this IMeshableConverter converter, SMesh mesh, GameObject obj)
        {
            if (mesh == null || mesh.vertices.Count == 0 || mesh.faces.Count == 0) return;

            var data = new SpeckleMesh.Data
            {
                uvs = new List<Vector2>(),
                vertices = new List<Vector3>(),
                subMeshes = new List<List<int>>(),
                vertexColors = new List<Color>()
            };

            var nativeMesh = new UnityEngine.Mesh();

            data.AddMesh(mesh);

            nativeMesh.SetVertices(data.vertices);
            nativeMesh.SetUVs(0, data.uvs);
            nativeMesh.SetColors(data.vertexColors);

            var j = 0;
            foreach (var subMeshTriangles in data.subMeshes)
            {
                nativeMesh.SetTriangles(subMeshTriangles, j);
                j++;
            }

            if (nativeMesh.vertices.Length >= ushort.MaxValue)
                nativeMesh.indexFormat = IndexFormat.UInt32;

            nativeMesh.Optimize();
            nativeMesh.RecalculateBounds();
            nativeMesh.RecalculateNormals();
            nativeMesh.RecalculateTangents();

            nativeMesh.subMeshCount = data.subMeshes.Count;

            var filter = obj.GetComponent<MeshFilter>();

            if (filter == null)
                filter = obj.AddComponent<MeshFilter>();

            if (IsRuntime)
                filter.mesh = nativeMesh;
            else
                filter.sharedMesh = nativeMesh;

            if (converter.addMeshCollider)
            {
                var c = filter.gameObject.GetComponent<MeshCollider>();
                if (c == null) c = filter.gameObject.AddComponent<MeshCollider>();

                c.sharedMesh = IsRuntime ? filter.mesh : filter.sharedMesh;
            }

            var meshRenderer = filter.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null) meshRenderer = filter.gameObject.AddComponent<MeshRenderer>();

            // c.sharedMaterial = converter.useRenderMaterial ?
            //   GetMaterial(converter, mesh["renderMaterial"] as RenderMaterial) :
            //   converter.defaultMaterial;
        }


        public static GameObject MeshToNative(this IMeshableConverter converter, IReadOnlyCollection<SMesh> meshes, GameObject obj)
        {
            var materials = new List<UnityEngine.Material>(meshes.Count);

            var data = new SpeckleMesh.Data
            {
                uvs = new List<Vector2>(),
                vertices = new List<Vector3>(),
                subMeshes = new List<List<int>>(),
                vertexColors = new List<Color>()
            };

            foreach (var speckleMesh in meshes)
            {
                if (speckleMesh.vertices.Count == 0 || speckleMesh.faces.Count == 0) continue;

                data.AddMesh(speckleMesh);
                // // Convert RenderMaterial
                // materials.Add(converter.useRenderMaterial ?
                //   GetMaterial(converter, speckleMesh["renderMaterial"] as RenderMaterial) :
                //   converter.defaultMaterial
                // );
            }

            var nativeMaterials = materials.ToArray();

            var nativeMesh = new UnityEngine.Mesh {subMeshCount = data.subMeshes.Count};

            nativeMesh.SetVertices(data.vertices);
            nativeMesh.SetUVs(0, data.uvs);
            nativeMesh.SetColors(data.vertexColors);

            var j = 0;
            foreach (var subMeshTriangles in data.subMeshes)
            {
                nativeMesh.SetTriangles(subMeshTriangles, j);
                j++;
            }

            if (nativeMesh.vertices.Length >= ushort.MaxValue)
                nativeMesh.indexFormat = IndexFormat.UInt32;

            nativeMesh.Optimize();
            nativeMesh.RecalculateBounds();
            nativeMesh.RecalculateNormals();
            nativeMesh.RecalculateTangents();

            // Debug.Log($"Mesh Stats for Native"
            //           + $"\nVertexCount:{nativeMesh.vertexCount}"
            //           + $"\nSubMeshes:{nativeMesh.subMeshCount}");

            var filter = obj.GetComponent<MeshFilter>();

            if (filter == null)
                filter = obj.AddComponent<MeshFilter>();

            if (IsRuntime)
                filter.mesh = nativeMesh;
            else
                filter.sharedMesh = nativeMesh;

            if (converter.addMeshCollider)
                filter.gameObject.AddComponent<MeshCollider>().sharedMesh = IsRuntime ? filter.mesh : filter.sharedMesh;

            return obj;
        }

        public static void AddMesh(this SpeckleMesh.Data data, SMesh subMesh)
        {
            subMesh.AlignVerticesWithTexCoordsByIndex();
            subMesh.TriangulateMesh();

            var indexOffset = data.vertices.Count;

            // Convert Vertices
            data.vertices.AddRange(subMesh.vertices.ArrayToVector3(subMesh.units));

            // Convert texture coordinates
            var hasValidUVs = subMesh.TextureCoordinatesCount == subMesh.VerticesCount;
            if (subMesh.textureCoordinates.Count > 0 && !hasValidUVs)
                Debug.LogWarning(
                    $"Expected number of UV coordinates to equal vertices. Got {subMesh.TextureCoordinatesCount} expected {subMesh.VerticesCount}. \nID = {subMesh.id}");

            if (hasValidUVs)
            {
                data.uvs.Capacity += subMesh.TextureCoordinatesCount;
                for (var j = 0; j < subMesh.TextureCoordinatesCount; j++)
                {
                    var (u, v) = subMesh.GetTextureCoordinate(j);
                    data.uvs.Add(new Vector2((float)u, (float)v));
                }
            }
            else if (subMesh.bbox != null)
            {
                //Attempt to generate some crude UV coordinates using bbox
                ////TODO this will be broken for submeshes
                data.uvs.AddRange(subMesh.bbox.GenerateUV(data.vertices));
            }

            // Convert vertex colors
            if (subMesh.colors != null)
            {
                if (subMesh.colors.Count == subMesh.VerticesCount)
                    data.vertexColors.AddRange(subMesh.colors.Select(c => c.ToUnityColor()));
                else if (subMesh.colors.Count != 0)
                    //TODO what if only some submeshes have colors?
                    Debug.LogWarning(
                        $"{typeof(SMesh)} {subMesh.id} has invalid number of vertex {nameof(SMesh.colors)}. Expected 0 or {subMesh.VerticesCount}, got {subMesh.colors.Count}");
            }

            var tris = new List<int>();

            // Convert faces
            tris.Capacity += (int)(subMesh.faces.Count / 4f) * 3;

            // skip the 0 index and then only grab every 3 
            for (var i = 0; i < subMesh.faces.Count; i += 4)
            {
                //We can safely assume all faces are triangles since we called TriangulateMesh
                tris.Add(subMesh.faces[i + 1] + indexOffset);
                tris.Add(subMesh.faces[i + 3] + indexOffset);
                tris.Add(subMesh.faces[i + 2] + indexOffset);
            }

            data.subMeshes.Add(tris);
        }

        public static IEnumerable<Vector2> GenerateUV(this Box bbox, IReadOnlyList<Vector3> verts)
        {
            var uv = new Vector2[verts.Count];
            var xSize = (float)bbox.xSize.Length;
            var ySize = (float)bbox.ySize.Length;

            for (var i = 0; i < verts.Count; i++)
            {
                var vert = verts[i];
                uv[i] = new Vector2(vert.x / xSize, vert.y / ySize);
            }

            return uv;
        }

        public static void RecenterVertices(List<Vector3> vertices, out Vector3 center)
        {
            center = Vector3.zero;

            if (vertices == null || !vertices.Any()) return;

            Bounds meshBounds = new Bounds {center = vertices[0]};

            foreach (var vert in vertices)
                meshBounds.Encapsulate(vert);

            center = meshBounds.center;

            for (int i = 0; i < vertices.Count; i++)
                vertices[i] -= meshBounds.center;
        }

    }

}
