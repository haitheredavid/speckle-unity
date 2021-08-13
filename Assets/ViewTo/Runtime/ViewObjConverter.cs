using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConnectorUnity;
using ConnectorUnity.Converters;
using Sentry;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using UnityEngine;
using ViewTo.Connector.Unity;
using ViewTo.Objects.Speckle;
using ViewTo.Objects.Structure;

namespace Speckle.ConnectorUnity
{

  public class ViewObjConverter : MonoConverter<ViewToKit>
  {
    private ViewObjConverterScript _converter;
    private ConverterUnity _basicConverter;

    public string app { get; set; }

    private GameObject ConvertToViewCloud(Base @base)
    {
      var pts = @base["points"];
      return null;
    }

    public GameObject ConvertRecursivelyToUnity(Base @base)
    {
      _converter ??= LoadConverter<ViewObjConverterScript>(app);
      if (_converter != null)
      {
        Debug.Log($"Trying to base to {@base.TypeName()}");
        var o = _converter.ConvertToNative(@base);
        if (o != null)
          return o.ConvertToViewMono().gameObject;
      }

      return null;
    }

    private GameObject RecurseTreeToNative(object @object)
    {
      if (IsList(@object))
      {
        var list = ((IEnumerable)@object).Cast<object>();
        var objects = list.Select(RecurseTreeToNative).Where(x => x != null).ToList();

        if (objects.Any())
        {
          var go = new GameObject
          {
            name = "List"
          };
          objects.ForEach(x => x.transform.parent = go.transform);
          return go;
        }
      }
      else
      {
        return TryConvertItemToNative(@object);
      }
      return null;
    }

    #region Copy Pasta from speckle
    private GameObject TryConvertItemToNative(object value)
    {
      if (value == null)
        return null;

      //it's a simple type or not a Base
      if (value.GetType().IsSimpleType() || !(value is Base))
      {
        return null;
      }

      var @base = (Base)value;

      //it's an unsupported Base, go through each of its property and try convert that
      if (!_converter.CanConvertToNative(@base))
      {
        var members = @base.GetMemberNames().ToList();

        //empty game object with the commit id as name, used to contain all the rest
        var go = new GameObject();
        go.name = @base.speckle_type;
        var goos = new List<GameObject>();
        foreach (var member in members)
        {
          var goo = RecurseTreeToNative(@base[member]);
          if (goo != null)
          {
            goo.name = member;
            goo.transform.parent = go.transform;
            goos.Add(goo);
          }
        }

        //if no children is valid, return null
        if (!goos.Any())
        {
          ConnectorUtilities.SafeDestroy(go);
          return null;
        }

        return go;
      }
      else
      {
        try
        {
          var go = _converter.ConvertToNative(@base) as GameObject;
          // Some revit elements have nested elements in a "elements" property
          // for instance hosted families on a wall
          if (go != null && @base["elements"] is List<Base> l && l.Any())
          {
            var goo = RecurseTreeToNative(l);
            if (goo != null)
            {
              goo.name = "elements";
              goo.transform.parent = go.transform;
            }
          }

          return go;
        }
        catch (Exception e)
        {
          throw new SpeckleException(e.Message, e, true, SentryLevel.Error);
        }
      }
    }

    private static bool IsList(object @object)
    {
      if (@object == null)
        return false;

      var type = @object.GetType();
      return typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IDictionary).IsAssignableFrom(type) && type != typeof(string);
    }

    private static bool IsDictionary(object @object)
    {
      if (@object == null)
        return false;

      Type type = @object.GetType();
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }
    #endregion

  }

}