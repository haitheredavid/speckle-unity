using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Objects;
using Objects.BuiltElements;
using Objects.Geometry;
using Sentry;
using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using UnityEngine;
using Mesh = Objects.Geometry.Mesh;

namespace Speckle.ConnectorUnity
{

  [CreateAssetMenu(fileName = "UnityConverter", menuName = "Speckle/Speckle Unity Converter", order = -1)]
  public partial class ConverterUnity : ScriptableObject, ISpeckleConverter
  {

    [Header("Speckle Converter Informations")]
    [SerializeField] protected string description;
    [SerializeField] protected string author;
    [SerializeField] protected string websiteOrEmail;

    [Space]
    [Header("Standard Unity Things")]
    [SerializeField] protected bool useRenderMaterial;

    #region converters
    [Space]
    [Header("Component Converters")]
    // [SerializeField] protected ComponentConverterBase defaultConverter;
    [SerializeField] protected ComponentConverterMesh meshConverter;
    [SerializeField] protected ComponentConverterPolyline polylineConverter;
    [SerializeField] protected ComponentConverterPoint pointConverter;
    [SerializeField] protected ComponentConverterPointCloud cloudConverter;
    [SerializeField] protected ComponentConverterView3D view3DConverter;
    #endregion

    [Space]
    [SerializeField] private List<ComponentConverter> otherConverters;

    #region converter properties
    public string Name => name;

    public string Description => description;

    public string Author => author;

    public string WebsiteOrEmail => websiteOrEmail;

    /// <summary>
    /// Default Unity units are in meters
    /// </summary>
    public string ModelUnits => Units.Meters;
    #endregion converter properties

    public ProgressReport Report { get; }

    public IEnumerable<string> GetServicedApplications() => new[] { HostApplications.Unity.Name };

    public HashSet<Exception> ConversionErrors { get; } = new HashSet<Exception>();

    public List<ApplicationPlaceholderObject> ContextObjects { get; set; } = new List<ApplicationPlaceholderObject>();

    public virtual void SetContextObjects(List<ApplicationPlaceholderObject> objects) => ContextObjects = objects;

    public virtual void SetContextDocument(object doc)
    {
      Debug.Log("Empty call from SetContextDocument");
    }

    public virtual void SetPreviousContextObjects(List<ApplicationPlaceholderObject> objects)
    {
      Debug.Log("Empty call from SetPreviousContextObjects");
    }

    public virtual void SetConverterSettings(object settings)
    {
      Debug.Log($"Converter Settings being set with {settings}");
    }

    public virtual Base ConvertToSpeckle(object @object)
    {

      if (converters == null || !converters.Any())
        CompileConverters(false);

      // convert for unity types
      // we have to figure out what is being passed into there.. it could be a single component that we want to convert
      // or it could be root game object with children we want to handle... for now we will assume this is handled in the loop checks from the client objs
      // or it can be a game object with multiple components that we want to convert
      List<Component> comps = new List<Component>();
      switch (@object)
      {
        case GameObject o:
          comps = o.GetComponents(typeof(Component)).ToList();
          break;
        case Component o:
          comps = new List<Component>() { o };
          break;
        case null:
          Debug.LogWarning("Trying to convert null object to speckle");
          break;
        default:
          Debug.LogException(new SpeckleException($"Native unity object {@object.GetType()} is not supported"));
          break;
      }

      if (!comps.Any())
      {
        Debug.LogWarning("No comps were found in the object trying to be covnerted :(");
        return null;
      }


      // TODO : handle when there is multiple convertable object types on game object
      foreach (var comp in comps)
      {
        var type = comp.GetType().ToString();

        foreach (var pair in converters)
        {
          if (pair.Key.Equals(type))
            return pair.Value.ToSpeckle(comp);
        }
      }

      Debug.LogWarning("No components found for converting to speckle");
      return null;

    }

    private Dictionary<string, ComponentConverter> converters;

    private void CompileConverters(bool toUnity = true)
    {
      converters = new Dictionary<string, ComponentConverter>()
      {
        { meshConverter.targetType(toUnity), meshConverter },
        { polylineConverter.targetType(toUnity), polylineConverter },
        { cloudConverter.targetType(toUnity), cloudConverter },
        { pointConverter.targetType(toUnity), pointConverter },
        { view3DConverter.targetType(toUnity), view3DConverter }
      };

      if (otherConverters != null && otherConverters.Any())
        foreach (var c in otherConverters)
          converters.Add(c.targetType(toUnity), c);

      foreach (var c in converters.Values)
      {
        if (c is IWantContextObj wanter)
          wanter.contextObjects = ContextObjects;
      }
    }

    public virtual object ConvertToNative(Base @base)
    {
      if (@base == null)
      {
        Debug.LogWarning("Trying to convert a null object! Beep Beep! I don't like that");
        return null;
      }

      foreach (var pair in converters)
      {
        if (pair.Key.Equals(@base.speckle_type))
          return pair.Value.ToNative(@base);
      }

      Debug.Log($"No Converters were found to handle {@base.speckle_type}, trying for display value");

      if (@base["displayValue"] is Mesh mesh)
      {
        Debug.Log("Handling Singluar Display Value");
        return meshConverter.ToNative(mesh);
      }

      if (@base["displayValue"] is IEnumerable<Base> bs)
      {
        Debug.Log("Handling List of Display Value");

        var go = new GameObject(@base.speckle_type);
        var potDisplayValues = RecurseTreeToNative(bs, "DisplayValues");

        if (potDisplayValues != null)
          potDisplayValues.transform.SetParent(go.transform);

        return go;
      }

      Debug.LogWarning($"Skipping {@base.GetType()} {@base.id} - Not supported type");
      return null;
    }

    public GameObject ConvertRecursively(object value)
    {
      if (value == null)
        return null;

      if (converters == null || !converters.Any())
        CompileConverters();

      //it's a simple type or not a Base
      if (value.GetType().IsSimpleType() || !(value is Base @base)) return null;

      return CanConvertToNative(@base) ?
        TryConvertToNative(@base) : // supported object so convert that 
        TryConvertProperties(@base); // not supported but might have props

    }

    private GameObject TryConvertToNative(Base @base)
    {
      try
      {
        var go = ConvertToNative(@base) as GameObject;

        if (go == null)
        {
          Debug.LogWarning("Object was not converted correclty");
          return null;
        }

        if (HasElements(@base, out var elements))
        {
          var goo = RecurseTreeToNative(elements, "Elements");

          if (goo != null)
            goo.transform.SetParent(go.transform);
        }
        return go;
      }
      catch (Exception e)
      {
        Debug.LogException(new SpeckleException(e.Message, e, true, SentryLevel.Error));
        return null;
      }
    }

    private GameObject TryConvertProperties(Base @base)
    {
      var go = new GameObject(@base.speckle_type);

      // go.AddComponent<SpeckleStream>();

      var props = new List<GameObject>();

      foreach (var prop in @base.GetMemberNames().ToList())
      {
        var goo = RecurseTreeToNative(@base[prop]);
        if (goo != null)
        {
          goo.name = prop;
          goo.transform.SetParent(go.transform);
          props.Add(goo);
        }
      }

      //if no children is valid, return null
      if (!props.Any())
      {
        Utils.SafeDestroy(go);
        return null;
      }

      return go;
    }

    private GameObject RecurseTreeToNative(object @object, string containerName = null)
    {
      if (!IsList(@object))
        return ConvertRecursively(@object);

      var list = ((IEnumerable)@object).Cast<object>();

      var go = new GameObject(containerName.Valid() ? containerName : "List");

      var objects = list.Select(x => RecurseTreeToNative(x)).Where(x => x != null).ToList();

      if (objects.Any())
        objects.ForEach(x => x.transform.SetParent(go.transform));

      return go;
    }

    public List<Base> ConvertToSpeckle(List<object> objects) => objects.Select(ConvertToSpeckle).ToList();

    public List<object> ConvertToNative(List<Base> objects) => objects.Select(ConvertToNative).ToList();

    public virtual bool CanConvertToSpeckle(object @object)
    {
      switch (@object)
      {
        case GameObject o:
          return o.GetComponent<MeshFilter>() != null;
        default:
          return false;
      }
    }

    public virtual bool CanConvertToNative(Base @object)
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
        case IDisplayValue<Mesh> _:
          return true;
        case Mesh _:
          return true;
        default:
          return @object["displayMesh"] is Mesh;
      }
    }

    #region static methods
    private static bool HasElements(Base @base, out List<Base> items)
    {
      items = null;

      if (@base["elements"] is List<Base> l && l.Any())
        items = l;

      return items != null;
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
    #endregion

  }

}