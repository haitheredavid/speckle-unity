using System;
using Objects.Geometry;
using UnityEngine;

namespace Objects.Converter.Unity
{
  [CreateAssetMenu(fileName = "PointCloudConverter", menuName = "Speckle/PointCloud Converter")]
  public class ComponentConverterPointCloud : ComponentConverter<Pointcloud>
  {

    protected override Component Process(Pointcloud @base)
    {
      throw new NotImplementedException();
    }
  }

}