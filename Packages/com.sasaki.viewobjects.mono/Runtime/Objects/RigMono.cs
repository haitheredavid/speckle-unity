using System.Collections.Generic;
using System.Linq;
using ViewTo.Objects;
using ViewTo.Objects.Structure;

namespace ViewTo.Connector.Unity
{
  public class RigMono : ViewObjBehaviour<RigObj>
  {
    
    public List<CloudShell> points
    {
      get => viewObj.clouds != null && viewObj.clouds.Any() ? viewObj.clouds.ToUnity() : null;
      set => viewObj.clouds = value.ToView();
    }

    public List<RigParameters> globalBundles
    {
      get => viewObj.globalParams;
      set => viewObj.globalParams = value;
    }

    public List<ViewColor> globalColors
    {
      get => viewObj.globalColors;
      set => viewObj.globalColors = value;
    }

    public List<RigParametersIsolated> isolatedBundles
    {
      get => viewObj.isolatedParams;
      set => viewObj.isolatedParams = value;
    }
    
    // public override RigObj CopyObj()
    // {
    //   
    //   throw new System.NotImplementedException();
    // }
  }
}