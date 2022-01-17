﻿// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Speckle.Core.Kits;
// using Speckle.Core.Models;
// using UnityEngine;
// using Object = UnityEngine.Object;
//
// namespace Objects.Converter.Unity
// {
//
//
//   public class BasicConverter : ScriptableObject, ISpeckleConverter
//   {
//
//     public ConverterComponent meshConversion;
//     
//     
//     #region implemented methods
//     public string Description => "Default Speckle Kit for Unity";
//     public string Name => nameof(ConverterUnity);
//     public string Author => "Speckle";
//     public string WebsiteOrEmail => "https://speckle.systems";
//
//     public ProgressReport Report { get; }
//
//     public IEnumerable<string> GetServicedApplications() => new[] { Applications.Unity }; //TODO: add unity
//
//     public HashSet<Exception> ConversionErrors { get; private set; } = new HashSet<Exception>();
//
//     public List<ApplicationPlaceholderObject> ContextObjects { get; set; } = new List<ApplicationPlaceholderObject>();
//
//     public void SetContextDocument(object doc) => throw new NotImplementedException();
//
//     public void SetContextObjects(List<ApplicationPlaceholderObject> objects) => ContextObjects = objects;
//
//     public void SetPreviousContextObjects(List<ApplicationPlaceholderObject> objects) => throw new NotImplementedException();
//     
//     public Base ConvertToSpeckle(object @object)
//     {
//       switch (@object)
//       {
//         case GameObject o:
//           if (o.GetComponent<MeshFilter>() != null)
//             return MeshToSpeckle(o);
//
//           throw new NotSupportedException();
//         default:
//           throw new NotSupportedException();
//       }
//     }
//
//     public object ConvertToNative(Base @object)
//     {
//       switch (@object)
//       {
//         // case Point o:
//         //   return PointToNative(o);
//         // case Line o:
//         //   return LineToNative(o);
//         // case Polyline o:
//         //   return PolylineToNative(o);
//         // case Curve o:
//         //   return CurveToNative(o);
//         // case View3D o:
//         //   return View3DToNative(o);
//         case Mesh o:
//           return MeshToNative(o);
//         //Built elements with a mesh representation implement this interface
//         case IDisplayMesh o:
//           return MeshToNative((Base)o);
//         default:
//           //capture any other object that might have a mesh representation
//           if (@object["displayMesh"] is Mesh)
//             return MeshToNative(@object["displayMesh"] as Mesh);
//
//           throw new NotSupportedException();
//       }
//     }
//
//     public List<Base> ConvertToSpeckle(List<object> objects)
//     {
//       return objects.Select(x => ConvertToSpeckle(x)).ToList();
//     }
//
//     public List<object> ConvertToNative(List<Base> objects)
//     {
//       return objects.Select(x => ConvertToNative(x)).ToList();
//
//       ;
//     }
//
//     public bool CanConvertToSpeckle(object @object)
//     {
//       switch (@object)
//       {
//         case GameObject o:
//           return o.GetComponent<MeshFilter>() != null;
//         default:
//           return false;
//       }
//     }
//
//     public bool CanConvertToNative(Base @object)
//     {
//       switch (@object)
//       {
//         // case Point _:
//         //   return true;
//         // case Line _:
//         //   return true;
//         // case Polyline _:
//         //   return true;
//         // case Curve _:
//         //   return true;
//         // case View3D _:
//         //   return true;
//         // case View2D _:
//         //   return false;
//         case IDisplayMesh _:
//           return true;
//         case Mesh _:
//           return true;
//         default:
//           return @object["displayMesh"] is Mesh;
//       }
//     }
//     #endregion implemented methods
//
//   }
// }

