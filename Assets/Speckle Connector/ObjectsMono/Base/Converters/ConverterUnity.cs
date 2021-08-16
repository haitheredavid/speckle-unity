using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects;
using Objects.Geometry;
using Sentry;
using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using UnityEngine;
using Mesh = Objects.Geometry.Mesh;

namespace ConnectorUnity
{

  public interface IConvertMono
  {
    public GameObject ConvertRecursivelyToNative(Base @base);
  }

  public partial class ConverterUnity : ISpeckleConverter, IConvertMono
  {

    #region implemented methods
    public string Description => "Default Speckle Kit for Unity";
    public string Name => nameof(ConverterUnity);
    public string Author => "Speckle";
    public string WebsiteOrEmail => "https://speckle.systems";

    public IEnumerable<string> GetServicedApplications() => new string[] {Applications.Other}; //TODO: add unity

    public HashSet<Exception> ConversionErrors { get; private set; } = new HashSet<Exception>();

    public List<ApplicationPlaceholderObject> ContextObjects { get; set; } = new List<ApplicationPlaceholderObject>();

    public void SetContextDocument(object doc) => throw new NotImplementedException();

    public void SetContextObjects(List<ApplicationPlaceholderObject> objects) => ContextObjects = objects;

    public void SetPreviousContextObjects(List<ApplicationPlaceholderObject> objects) => throw new NotImplementedException();

    public Base ConvertToSpeckle(object @object)
    {
      switch (@object)
      {
        case GameObject o:
          if (o.GetComponent<MeshFilter>() != null)
            return MeshToSpeckle(o);

          throw new NotSupportedException();
        default:
          throw new NotSupportedException();
      }
    }

    public object ConvertToNative(Base @object)
    {
      switch (@object)
      {
        // case Point o:
        //   return PointToNative(o);
        // case Line o:
        //   return LineToNative(o);
        // case Polyline o:
        //   return PolylineToNative(o);
        // case Curve o:
        //   return CurveToNative(o);
        // case View3D o:
        //   return View3DToNative(o);
        case Pointcloud o:
          return PointCloudToNative(o);
        case Mesh o:
          return MeshToNative(o);
        //Built elements with a mesh representation implement this interface
        case IDisplayMesh o:
          return MeshToNative((Base)o);
        default:
          //capture any other object that might have a mesh representation
          if (@object["displayMesh"] is Mesh)
            return MeshToNative(@object["displayMesh"] as Mesh);

          throw new NotSupportedException();
      }
    }

    public List<Base> ConvertToSpeckle(List<object> objects)
    {
      return objects.Select(x => ConvertToSpeckle(x)).ToList();
    }

    public List<object> ConvertToNative(List<Base> objects)
    {
      return objects.Select(x => ConvertToNative(x)).ToList();
    }

    public bool CanConvertToSpeckle(object @object)
    {
      switch (@object)
      {
        case GameObject o:
          return o.GetComponent<MeshFilter>() != null;
        default:
          return false;
      }
    }

    public bool CanConvertToNative(Base @object)
    {
      switch (@object)
      {
        // case Point _:
        //   return true;
        // case Line _:
        //   return true;
        // case Polyline _:
        //   return true;
        // case Curve _:
        //   return true;
        // case View3D _:
        //   return true;
        // case View2D _:
        //   return false;
        case Pointcloud _:
          return true;
        case IDisplayMesh _:
          return true;
        case Mesh _:
          return true;
        default:
          return @object["displayMesh"] is Mesh;
      }
    }
    #endregion implemented methods

    /// <summary>
    /// Converts a Base object to a GameObject Recursively
    /// </summary>
    /// <param name="base"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public GameObject ConvertRecursivelyToNative(Base @base)
    {

      //using the ApplicationPlaceholderObject to pass materials
      //available in Assets/Materials to the converters
      var materials = Resources.LoadAll("Materials", typeof(Material)).Cast<Material>()
        .Select(x => new ApplicationPlaceholderObject {NativeObject = x}).ToList();

      SetContextObjects(materials);


      // case 1: it's an item that has a direct conversion method, eg a point
      if (CanConvertToNative(@base))
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
        go.name = members.First();
        return go;
      }
      else
      {
        //empty game object with the commit id as name, used to contain all the rest
        var go = new GameObject
        {
          name = @base.id.Valid() ? @base.id : "@Base"
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
    /// Converts an object recursively to a list of GameObjects
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
      if (value.GetType().IsSimpleType() || !(value is Base))
      {
        return null;
      }

      var @base = (Base)value;

      //it's an unsupported Base, go through each of its property and try convert that
      if (!CanConvertToNative(@base))
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
          var go = ConvertToNative(@base) as GameObject;
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
      return(typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IDictionary).IsAssignableFrom(type) && type != typeof(string));
    }

    private static bool IsDictionary(object @object)
    {
      if (@object == null)
        return false;

      Type type = @object.GetType();
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }
  }
}