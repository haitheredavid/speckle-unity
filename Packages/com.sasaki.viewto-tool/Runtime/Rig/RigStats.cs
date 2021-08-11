using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ViewToUnity
{

  public class RigStats
  {

    public enum Stage
    {

      Target,
      Blocker,
      Design,
      Complete

    }

    private const int FrameRateCount = 10;

    private readonly List<int> _frameRates = new List<int>();

    public int GetRate
    {
      get
      {
        var frame = (int)Math.Floor(1f / Time.unscaledDeltaTime);

        if (_frameRates.Count >= FrameRateCount)
          _frameRates.RemoveAt(0);

        _frameRates.Add(frame);

        var smoothedFrameCount = _frameRates.Sum();

        smoothedFrameCount /= _frameRates.Count;
        return smoothedFrameCount;
      }
    }
  }
}