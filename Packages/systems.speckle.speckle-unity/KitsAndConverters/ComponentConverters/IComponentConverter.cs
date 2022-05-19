using System.Collections.Generic;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity
{

  public interface IWantContextObj
  {
    public List<ApplicationPlaceholderObject> contextObjects { set; }
  }

  public interface IComponentConverter
  {
    public bool CanConvertToNative(Base type);
    public bool CanConvertToSpeckle(Component type);

    public GameObject ToNative(Base @base);
    public Base ToSpeckle(Component component);

    public string speckle_type { get; }
    public string unity_type { get; }
  }
}