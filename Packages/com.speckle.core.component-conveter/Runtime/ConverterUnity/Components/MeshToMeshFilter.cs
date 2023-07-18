using System.Collections.Generic;
using System.Linq;
using Objects.Utils;
using Speckle.ConnectorUnity.Core.ScriptableConverter;
using Speckle.ConnectorUnity.Core.ScriptableConverter.Components;
using Speckle.Core.Models;
using UnityEngine;
using UnityEngine.Rendering;
using Umesh = UnityEngine.Mesh;
using Smesh = Objects.Geometry.Mesh;


namespace Speckle.ConnectorUnity.Converter
{

    [CreateAssetMenu(fileName = nameof(MeshToMeshFilter), menuName = SpeckleUnity.Categories.CONVERTERS + "Create Mesh Converter")]
    public class MeshToMeshFilter : ComponentConverter<Objects.Geometry.Mesh, MeshFilter>, IMeshableConverter, IWantContextObj
    {

        /// <inheritdoc />
        public List<ApplicationObject> contextObjects {get;set;}

        /// <inheritdoc />
        [field: SerializeField]
        public bool addMeshCollider {get;private set;}

        /// <inheritdoc />
        [field: SerializeField]
        public bool recenterTransform {get;private set;}

        /// <inheritdoc />
        [field: SerializeField]
        public bool useRenderMaterial {get;private set;}

        /// <inheritdoc />
        [field: SerializeField]
        public bool combineMeshes {get;private set;}

        /// <inheritdoc />
        [field: SerializeField]
        public Material defaultMaterial {get;private set;}

        protected override void OnEnable()
        {
            base.OnEnable();

            if (defaultMaterial == null) defaultMaterial = new UnityEngine.Material(Shader.Find("Standard"));
        }

        protected override MeshFilter CreateUnityInstance(Transform parent = null)
        {
            var instance = base.CreateUnityInstance(parent);
            
            var renderer = instance.gameObject.GetComponent<MeshRenderer>();
            if (renderer == null) renderer = instance.gameObject.AddComponent<MeshRenderer>();
            // c.sharedMaterial = converter.useRenderMaterial ?
            //   GetMaterial(converter, mesh["renderMaterial"] as RenderMaterial) :
            //   converter.defaultMaterial;
            
            if (addMeshCollider)
            {
                var collider = instance.gameObject.GetComponent<MeshCollider>();
                if (collider == null) collider = instance.gameObject.AddComponent<MeshCollider>();

                collider.sharedMesh = ConverterUtils.IsRuntime ? instance.mesh : instance.sharedMesh;
            }

           

            return instance;
        }


        protected override Smesh Deserialize(MeshFilter component)
        {
            var nativeMesh = ConverterUtils.IsRuntime ? component.mesh : component.sharedMesh;

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

            return new Smesh
            {
                vertices = sVertices,
                faces = sFaces,
                colors = sColors,
                textureCoordinates = sTexCoords,
                units = ConverterUtils.ModelUnits
            };

            return this.MeshToSpeckle(component);
        }

        protected override void Serialize(Smesh obj, MeshFilter target)
        {
            Debug.Log("Building Native From Mesh");

            if (obj == null || obj.vertices.Count == 0 || obj.faces.Count == 0) return;

            void AddMesh(Objects.Geometry.Mesh subMesh, ref SpeckleMesh.Data data)
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
                            $"{typeof(Smesh)} {subMesh.id} has invalid number of vertex {nameof(Smesh.colors)}. Expected 0 or {subMesh.VerticesCount}, got {subMesh.colors.Count}");
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

            Umesh ConvertMeshData(Objects.Geometry.Mesh b)
            {
                var data = new SpeckleMesh.Data
                {
                    uvs = new List<Vector2>(),
                    vertices = new List<Vector3>(),
                    subMeshes = new List<List<int>>(),
                    vertexColors = new List<Color>()
                };

                var nativeMesh = new Umesh();

                AddMesh(b, ref data);
                // data.AddMesh(b);

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
                return nativeMesh;
            }

            var nativeMesh = ConvertMeshData(obj);

            if (ConverterUtils.IsRuntime) target.mesh = nativeMesh;
            else target.sharedMesh = nativeMesh;
        }

    }

}
