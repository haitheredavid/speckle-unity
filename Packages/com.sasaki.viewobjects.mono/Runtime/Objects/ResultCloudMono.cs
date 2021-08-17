using System.Collections.Generic;
using ViewTo.Objects;
using ViewTo.Objects.Elements;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{
  public class ResultCloudMono : CloudBehaviour<ResultCloud>, IResultCloud<PixelData>
  {

    public List<PixelData> data { get; set; }
    
    
    // public override ResultCloud CopyObj() => throw new System.NotImplementedException();
  }
}