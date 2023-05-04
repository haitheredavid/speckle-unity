using System.Collections.Generic;
using Speckle.ConnectorUnity.Converter;

namespace Speckle.ConnectorUnity.Converter
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
