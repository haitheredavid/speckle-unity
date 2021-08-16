using System;
using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;

namespace ConnectorUnity
{
  [ExecuteAlways]
  public class SpeckleConverter : MonoBehaviour
  {
    private ISpeckleConverter _converter;

    public GameObject ConvertObjects(Base @base, ISpeckleConverter converter = default)
    {

      if (converter != default)
        _converter = converter;

      return _converter.ConvertToNative(@base) as GameObject;
    }

  }
}