using System.Linq;
using Objects.Geometry;
using Speckle.Core.Models;
using UnityEngine;
using Speckle.ConnectorUnity.Core.ScriptableConverter;

namespace Speckle.ConnectorUnity
{

    [CreateAssetMenu(fileName = nameof(PolylineComponentConverter), menuName = SpeckleUnity.Categories.CONVERTERS + "Create Polyline Converter")]
    public class PolylineComponentConverter : ComponentConverter<Polyline, LineRenderer>
    {
        public float diameter;

        /// <summary>
        ///   Converts a Speckle curve to a GameObject with a line renderer
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected override void BuildNative(Polyline obj, LineRenderer target)
        {
            target.SetupLineRenderer(obj.GetPoints().ArrayToVector3(obj.units).ToArray(), diameter);
        }

        public override Base ToSpeckle(LineRenderer component)
        {
            // TODO: check if this should use world or local scale
            var points = new Vector3[component.positionCount];
            component.GetPositions(points);
            return new Polyline(points.ToSpeckle());
        }
    }

}
