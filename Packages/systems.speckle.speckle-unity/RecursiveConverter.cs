using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects.Converter.Unity;
using Sentry;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity
{
  public class RecursiveConverter : MonoBehaviour
  {

    private ConverterUnity converter;

    /// <summary>
    ///   Converts a Base object to a GameObject Recursively
    /// </summary>
    /// <param name="base"></param>
    /// <param name="parentName"></param>
    /// <param name="objectConverter"></param>
    /// <returns></returns>
    public GameObject ConvertRecursivelyToNative(Base @base, string parentName, ConverterUnity objectConverter)
    {

      //using the ApplicationPlaceholderObject to pass materials
      //available in Assets/Materials to the converters
      var materials = Resources.LoadAll("", typeof(Material)).Cast<Material>().ToArray();
      if (materials.Length == 0) Debug.Log("To automatically assign materials to recieved meshes, materials have to be in the \'Assets/Resources\' folder!");
      var placeholderObjects = materials.Select(x => new ApplicationPlaceholderObject { NativeObject = x }).ToList();

      converter = objectConverter;
      converter.SetContextObjects(placeholderObjects);

      Debug.Log($"Converter set to {converter.Name}");

      // case 1: it's an item that has a direct conversion method, eg a point
      if (converter.CanConvertToNative(@base))
      {
        var go = TryConvertItemToNative(@base);
        return go;
      }


      // case 2: it's a wrapper Base
      //       2a: if there's only one member unpack it
      //       2b: otherwise return dictionary of unpacked members
      var members = @base.GetMemberNames().ToList();
      if (members.Count() == 1)
      {
        var go = RecurseTreeToNative(@base[members.First()]);
        if (go != null)
        {
          go.name = members.First();
          return go;
        }
        else
        {
          return null;
        }
      }
      else
      {
        //empty game object with the commit id as name, used to contain all the rest
        var go = new GameObject
        {
          name = parentName.Valid() ? parentName : "Base"
        };

        foreach (var member in members)
        {
          var goo = RecurseTreeToNative(@base[member]);
          if (goo != null)
          {
            goo.name = member;
            goo.transform.parent = go.transform;
          }
        }

        return go;
      }
    }

    /// <summary>
    ///   Converts an object recursively to a list of GameObjects
    /// </summary>
    /// <param name="object"></param>
    /// <returns></returns>
    private GameObject RecurseTreeToNative(object @object)
    {
      if (IsList(@object))
      {
        var list = ((IEnumerable)@object).Cast<object>();
        var objects = list.Select(x => RecurseTreeToNative(x)).Where(x => x != null).ToList();
        if (objects.Any())
        {
          var go = new GameObject();
          go.name = "List";
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

    private GameObject TryConvertItemToNative(object value)
    {
      if (value == null)
        return null;

      //it's a simple type or not a Base
      if (value.GetType().IsSimpleType() || !(value is Base)) return null;

      var @base = (Base)value;

      //it's an unsupported Base, go through each of its property and try convert that
      if (!converter.CanConvertToNative(@base))
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
          Utils.SafeDestroy(go);
          return null;
        }

        return go;
      }
      try
      {
        var res = converter.ConvertToNative(@base);
        
        if (res == null)
        {
          Debug.LogWarning("Object was not converted correclty");
          return null;
        }
        
        Debug.Log($"Resulted conversion to type={res.GetType()}");

        GameObject go = null;
        
        if (res is Component comp)
          go = comp.gameObject;
        else if (res is GameObject g)
          go = g;
        else
        {
          Debug.LogWarning("Unhandled object type being passed back from converter");
          return null;
        }
        
        CheckElements(go, @base);
        
        return go;
      }
      catch (Exception e)
      {
        Debug.LogException(new SpeckleException(e.Message, e, true, SentryLevel.Error));
        return null;
      }
    }
    
    // Some revit elements have nested elements in a "elements" property
    // for instance hosted families on a wall
    private void CheckElements(GameObject go, Base @base)
    {
      if (go != null && @base["elements"] is List<Base> l && l.Any())
      {
        var goo = RecurseTreeToNative(l);
        if (goo != null)
        {
          goo.name = "elements";
          goo.transform.SetParent(transform);
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

      var type = @object.GetType();
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }
  }
}