﻿using System.Collections.Generic;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter.Examples
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
