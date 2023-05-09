using System.Collections.Generic;
using UnityEngine;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter.Components
{

    public class SpeckleMesh : BaseBehaviour
    {

        public struct Data
        {
            public List<Vector2> uvs;

            public List<Color> vertexColors;

            public List<Vector3> vertices;

            public List<List<int>> subMeshes;

        }
    }

}
