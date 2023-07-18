using System.Collections.Generic;
using Speckle.ConnectorUnity.Converter;
using Speckle.ConnectorUnity.Core.ScriptableConverter;

namespace Speckle.ConnectorUnity
{

    public class ConverterUnity : ScriptableConverter
    {

        protected override List<ComponentConverter> GetDefaultConverters()
        {
           return new List<ComponentConverter>
            {
                CreateInstance<MeshToMeshFilter>(),
                CreateInstance<PolylineToLineRenderer>(),
                CreateInstance<PointToSpecklePoint>(),
                // CreateInstance<PointCloudComponentConverter>(),
                CreateInstance<View3DToCamera>(),
                // CreateInstance<BrepComponentConverter>()
            };
        }
    }

}
