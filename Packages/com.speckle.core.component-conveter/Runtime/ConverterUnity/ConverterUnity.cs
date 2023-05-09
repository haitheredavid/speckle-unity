using System.Collections.Generic;
using Speckle.ConnectorUnity.Core.ScriptableConverter;
using Speckle.ConnectorUnity.Core.ScriptableConverter.Examples;

namespace Speckle.ConnectorUnity
{

    public class ConverterUnity : ScriptableConverter
    {

        protected override List<ComponentConverter> GetDefaultConverters()
        {
           return new List<ComponentConverter>
            {
                // CreateInstance<MeshComponentConverter>(),
                CreateInstance<PolylineComponentConverter>(),
                CreateInstance<PointComponentConverter>(),
                // CreateInstance<PointCloudComponentConverter>(),
                CreateInstance<View3DComponentConverter>(),
                // CreateInstance<BrepComponentConverter>()
            };
        }
    }

}
