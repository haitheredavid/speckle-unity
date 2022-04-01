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

    #region converters
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

    public virtual object ConvertToNative(Base @base)
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
          return TryConvert(meshConverter, o);
        // if (meshConverter != null) // TODO: address converters needing dependency from unity
        // return meshConverter.ToComponent(o, useRenderMaterial ? GetMaterial((RenderMaterial)o["renderMaterial"]) : defaultMaterial);
        default:
          //capture any other object that might have a mesh representation
          if (@base["displayValue"] is Mesh mesh)
            return TryConvert(meshConverter, mesh);

          if (@base["displayValue"] is IEnumerable<Base> bs)
          {
            var go = new GameObject("List");
            foreach (var obj in bs.OfType<Mesh>())
            {
              var res = TryConvert(meshConverter, obj);
              if (res != null)
                res.transform.SetParent(go.transform);
            }
            return go;
          }

          if (@base["displayValue"] is Base b)
            return TryConvert(baseConverter, b);

          Debug.LogWarning($"Skipping {@base.GetType()} {@base.id} - Not supported type");
          return null;
      }
    }

    protected Component TryConvert<TBase>(ComponentConverter<TBase> converter, TBase @base) where TBase : Base
    {
      return converter != null ? converter.ToComponent(@base) : null;
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
        case IDisplayValue<Geometry.Mesh> _:
          return true;
        case Geometry.Mesh _:
          return true;
        default:
          return @object["displayMesh"] is Geometry.Mesh;
      }
    }

  }
}