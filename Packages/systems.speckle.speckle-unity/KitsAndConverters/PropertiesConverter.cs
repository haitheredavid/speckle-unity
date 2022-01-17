using Speckle.ConnectorUnity;
using Speckle.Core.Models;

namespace Objects.Converter.Unity
{
  
  public class PropertiesConverter : ConverterComponent
  {
    public virtual SpeckleProperties ToComponent(Base @base) => new SpeckleProperties
    {
      Data = FetchProps(@base)
    };
  }
}