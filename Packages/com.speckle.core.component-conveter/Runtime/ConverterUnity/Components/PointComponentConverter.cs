using Objects.Geometry;
using Speckle.ConnectorUnity.Core.ScriptableConverter.Components;
using Speckle.Core.Models;
using UnityEngine;
using Speckle.ConnectorUnity.Core.ScriptableConverter;

namespace Speckle.ConnectorUnity
{
	[CreateAssetMenu(fileName = nameof(PointComponentConverter), menuName = SpeckleUnity.Categories.CONVERTERS + "Create Point Converter")]
	public class PointComponentConverter : ComponentConverter<Point, SpecklePoint>
	{

		protected override void ToNative(Point obj, ref SpecklePoint instance)
    {
        // instance.pos = obj.ToVector3();
    }

    public override Base ToSpeckle(SpecklePoint component)
    {
        // return component.pos.ToSpeckle();
        return null;
    }
  }
}
