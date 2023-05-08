using Objects.Geometry;
using Speckle.ConnectorUnity.Core.ScriptableConverter.Components;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter.Examples
{
	[CreateAssetMenu(fileName = nameof(PointComponentConverter), menuName = SpeckleUnity.Categories.CONVERTERS + "Create Point Converter")]
	public class PointComponentConverter : ComponentConverter<Point, SpecklePoint>
	{

		protected override void ConvertBase(Point obj, ref SpecklePoint instance)
    {
        // instance.pos = obj.ToVector3();
    }

    public override Base ConvertComponent(SpecklePoint component)
    {
        // return component.pos.ToSpeckle();
        return null;
    }
  }
}
