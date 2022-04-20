﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Objects.Geometry;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;
using Mesh = UnityEngine.Mesh;
using Object = UnityEngine.Object;

namespace Speckle.ConnectorUnity
{
  public static partial class ConverterUtils
  {
    /// <summary>
    ///   the default Unity units are meters
    /// </summary>
    public const string ModelUnits = Units.Meters;

    public static Dictionary<string, object> FetchProps<TBase>(this TBase @base) where TBase : Base
    {
      var props = typeof(TBase).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(x => x.Name).ToList();
      return @base.GetMembers().Where(x => !props.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
    }

    public static void AttachUnityProperties(this SpeckleProperties props, Base @base)
    {
      if (@base == null || props?.Data == null)
        return;

      foreach (var key in props.Data.Keys)
        @base[key] = props.Data[key];
    }

    public static double ScaleToNative(double value, string units)
    {
      return value * Units.GetConversionFactor(units, ModelUnits);
    }

    public static bool Valid(this string name)
    {
      return!string.IsNullOrEmpty(name);
    }

    public static int ToIntColor(this Color c) => System.Drawing.Color
      .FromArgb(Convert.ToInt32(c.r * 255), Convert.ToInt32(c.r * 255), Convert.ToInt32(c.r * 255))
      .ToArgb();

    public static Color ToUnityColor(this int c)
    {
      var argb = System.Drawing.Color.FromArgb(c);
      return new Color(argb.R / 255.0f, argb.G / 255.0f, argb.B / 255.0f);
    }
  }
}