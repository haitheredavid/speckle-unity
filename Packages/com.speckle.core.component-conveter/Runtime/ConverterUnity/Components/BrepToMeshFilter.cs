using Objects.Geometry;
using Speckle.ConnectorUnity.Core.ScriptableConverter;
using Speckle.ConnectorUnity.Core.ScriptableConverter.Components;
using UnityEngine;

namespace Speckle.ConnectorUnity
{

    [CreateAssetMenu(fileName = nameof(BrepToMeshFilter), menuName = SpeckleUnity.Categories.CONVERTERS + "Create Brep Converter")]
    public class BrepToMeshFilter: ComponentConverter<Brep, SpeckleBrep>
    {

        protected override void Serialize(Brep obj, SpeckleBrep target)
        {
            
            throw new System.NotImplementedException();
        }

        protected override Brep Deserialize(SpeckleBrep obj)
        {
            throw new System.NotImplementedException();
        }
    }

}
