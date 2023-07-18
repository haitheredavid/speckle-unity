using Objects.BuiltElements;
using Speckle.Core.Models;
using UnityEngine;
using Speckle.ConnectorUnity.Core.ScriptableConverter;

namespace Speckle.ConnectorUnity
{

    [CreateAssetMenu(fileName = nameof(View3DComponentConverter), menuName = SpeckleUnity.Categories.CONVERTERS + "Create View3d Converter")]
    public class View3DComponentConverter : ComponentConverter<View3D, Camera>
    {

        /// <summary>
        ///  Converts a Speckle View3D to a GameObject
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target"></param>
        protected override void Serialize(View3D obj, Camera target)
        {
            target.transform.position = ConverterUtils.VectorByCoordinates(
                obj.origin.x,
                obj.origin.y,
                obj.origin.z,
                obj.origin.units);

            target.transform.forward = ConverterUtils.VectorByCoordinates(
                obj.forwardDirection.x,
                obj.forwardDirection.y,
                obj.forwardDirection.z,
                obj.forwardDirection.units);

            target.transform.up = ConverterUtils.VectorByCoordinates(
                obj.upDirection.x,
                obj.upDirection.y,
                obj.upDirection.z,
                obj.upDirection.units);
        }

        protected override View3D Deserialize(Camera component)
        {
            return new View3D
            {
                origin = component.transform.position.ToPoint(),
                forwardDirection = component.transform.forward.ToSpeckle(),
                upDirection = component.transform.up.ToSpeckle(),
                isOrthogonal = component.orthographic
            };
        }
    }

}
