namespace Speckle.ConnectorUnity.Core.ScriptableConverter
{


    public interface IMeshableConverter
    {
        /// <summary>
        /// Adds mesh colliders to each mesh object
        /// </summary>
        public bool addMeshCollider
        {
            get;
        }
        
        /// <summary>
        /// Adds a mesh renderer to each object, helpful if you want to see what you're looking at
        /// </summary>
        public bool addMeshRenderer
        {
            get;
        }
        /// <summary>
        /// Repositions the origin of the mesh object to the center of the mesh bounds 
        /// </summary>
        public bool recenterTransform
        {
            get;
        }
        /// <summary>
        /// Reference the speckle mesh material for the mesh renderer
        /// </summary>
        public bool useRenderMaterial
        {
            get;
        }
        
        /// <summary>
        /// Default material for a speckle unity object
        /// </summary>
        public UnityEngine.Material defaultMaterial
        {
            get;
        }
        
        /// <summary>
        /// Combines all the meshes in a speckle object into a single instance. Useful for reducing draw calls
        /// </summary>
        public bool combineMeshes
        {
            get;
        }
    }

}
