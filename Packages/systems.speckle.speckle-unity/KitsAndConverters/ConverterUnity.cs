using System;
using System.Collections.Generic;
using System.Linq;
using Objects.BuiltElements;
using Objects.Geometry;
using Objects.Other;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;
using Mesh = Objects.Geometry.Mesh;

namespace Objects.Converter.Unity
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
    [SerializeField] protected Material defaultMaterial;

    #region Converters
    [Space]
    [Header("Component Converters")]
    [SerializeField] protected ComponentConverterBase baseConverter;
    [SerializeField] protected ComponentConverterMesh meshConverter;

    [SerializeField] protected ComponentConverterLine lineConverter;
    [SerializeField] protected ComponentConverterCurve curveConverter;
    [SerializeField] protected ComponentConverterPolyline polylineConverter;

    [SerializeField] protected ComponentConverterPoint pointConverter;
    [SerializeField] protected ComponentConverterPointCloud cloudConverter;
    [SerializeField] protected ComponentConverterView3D view3DConverter;
    #endregion

    #region converter properties
    public string Name => name;

    public string Description => description;

    public string Author => author;

    public string WebsiteOrEmail => websiteOrEmail;

    /// <summary>
    /// Default Unity units are in meters
    /// </summary>
    public string ModelUnits => Speckle.Core.Kits.Units.Meters;
    
    #endregion converter properties

    public ProgressReport Report { get; }

    public IEnumerable<string> GetServicedApplications() => new[] { Applications.Unity };

    public HashSet<Exception> ConversionErrors { get; } = new HashSet<Exception>();

    public List<ApplicationPlaceholderObject> ContextObjects { get; set; } = new List<ApplicationPlaceholderObject>();

    public void SetContextObjects(List<ApplicationPlaceholderObject> objects) => ContextObjects = objects;
    
    public void SetContextDocument(object doc)
    {
      Debug.Log("Empty call from SetContextDocument");
    }

    public void SetPreviousContextObjects(List<ApplicationPlaceholderObject> objects)
    {
      Debug.Log("Empty call from SetPreviousContextObjects");
    }

    public void SetConverterSettings(object settings)
    {
      Debug.Log($"Converter Settings being set with {settings}");
    }

    public void OnEnable()
    {
      defaultMaterial ??= new Material(Shader.Find("Standard"));
    }

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

    /// <summary>
    ///   Native objects return should return as Base Behaviour objects since these speckle properties
    /// </summary>
    /// <param name="base"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public object ConvertToNative(Base @base)
    {
      if (@base == null)
      {
        Debug.LogWarning("Trying to convert a null object! Beep Beep! I don't like that");
        return null;
      }

      switch (@base)
      {
        case Point o:
          return TryConvert(pointConverter, o);
        case Pointcloud o:
          return TryConvert(cloudConverter, o);
        case Line o:
          return TryConvert(lineConverter, o);
        case Polyline o:
          return TryConvert(polylineConverter, o);
        case Curve o:
          return TryConvert(curveConverter, o);
        case View3D o:
          return TryConvert(view3DConverter, o);
        case Mesh o:
          if (meshConverter != null)
            return meshConverter.ToComponent(o, useRenderMaterial ? GetMaterial((RenderMaterial)o["renderMaterial"]) : defaultMaterial);

          break;
        default:
          //capture any other object that might have a mesh representation
          if (@base["displayMesh"] is Mesh mesh)
            return TryConvert(meshConverter, mesh);

          break;
      }
      throw new Exception($"type of object {@base.speckle_type} is not supported with this converter");
    }

    protected Component TryConvert<TBase>(ComponentConverter<TBase> converter, TBase @base) where TBase : Base
    {
      return converter != null ? converter.ToComponent(@base) : null;
    }

    public List<Base> ConvertToSpeckle(List<object> objects) => objects.Select(ConvertToSpeckle).ToList();

    public List<object> ConvertToNative(List<Base> objects) => objects.Select(ConvertToNative).ToList();

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
        case IDisplayMesh _:
          return true;
        case Geometry.Mesh _:
          return true;
        default:
          return @object["displayMesh"] is Geometry.Mesh;
      }
    }

  }
}