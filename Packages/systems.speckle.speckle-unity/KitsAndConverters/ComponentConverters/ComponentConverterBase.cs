using Speckle.ConnectorUnity;
using Speckle.Core.Models;
using UnityEngine;

namespace Objects.Converter.Unity
{
  [CreateAssetMenu(fileName = "BaseConverter", menuName = "Speckle/Base Converter")]
  public class ComponentConverterBase : ComponentConverter<Base>
  {
    /// <summary>
    /// converts a base object with only properties 
    /// </summary>
    /// <param name="base"></param>
    /// <returns></returns>
    protected override Component Process(Base @base)
    {
      var bb = New<BaseBehaviour>();
      var obj = CreateInstance<SpeckleProperties>();
      obj.Data = @base.FetchProps();
      bb.properties = obj;
      return bb;
    }
  }
}