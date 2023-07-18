using Objects.Geometry;
using Speckle.ConnectorUnity.Core.ScriptableConverter.Components;
using Speckle.Core.Models;
using UnityEngine;
using Speckle.ConnectorUnity.Core.ScriptableConverter;

namespace Speckle.ConnectorUnity
{
	[CreateAssetMenu(fileName = nameof(PointToSpecklePoint), menuName = SpeckleUnity.Categories.CONVERTERS + "Create Point Converter")]
	public class PointToSpecklePoint : ComponentConverter<Point, SpecklePoint>
	{


      protected override void Serialize(Point obj, SpecklePoint target)
    {
        target.pos = obj.ToVector3();
    }

    protected override Point Deserialize(SpecklePoint obj)
    {
        return obj.pos.ToPoint();
    }
    
  }
}
