using System;
using System.Collections.Generic;
using Objects.Geometry;
using Objects.Other;
using Speckle.Core.Kits;
using UnityEditor;
using UnityEngine;
using Mesh = Objects.Geometry.Mesh;

namespace Speckle.ConnectorUnity
{
  public partial class ConverterUnity
  {

    private double ScaleToNative(double value, string units)
    {
      var f = Units.GetConversionFactor(units, ModelUnits);
      return value * f;
    }

    private Material GetMaterial(RenderMaterial renderMaterial)
    {
      //todo support more complex materials
      var shader = Shader.Find("Standard");
      var mat = new Material(shader);

      //if a renderMaterial is passed use that, otherwise try get it from the mesh itself

      if (renderMaterial != null)
      {
        // 1. match material by name, if any
        Material matByName = null;

        foreach (var _mat in ContextObjects)
          if (((Material)_mat.NativeObject).name == renderMaterial.name)
          {
            if (matByName == null) matByName = (Material)_mat.NativeObject;
            else Debug.LogWarning("There is more than one Material with the name \'" + renderMaterial.name + "\'!", (Material)_mat.NativeObject);
          }
        if (matByName != null) return matByName;

        // 2. re-create material by setting diffuse color and transparency on standard shaders
        if (renderMaterial.opacity < 1)
        {
          shader = Shader.Find("Transparent/Diffuse");
          mat = new Material(shader);
        }

        var c = renderMaterial.diffuse.ToUnityColor();
        mat.color = new Color(c.r, c.g, c.b, System.Convert.ToSingle(renderMaterial.opacity));
        mat.name = renderMaterial.name == null ? "material-" + Guid.NewGuid().ToString().Substring(0, 8) : renderMaterial.name;


        #if UNITY_EDITOR
        if (SpeckleConnector.GenerateMaterials)
        {
          if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");
          if (!AssetDatabase.IsValidFolder("Assets/Resources/Materials")) AssetDatabase.CreateFolder("Assets/Resources", "Materials");
          if (!AssetDatabase.IsValidFolder("Assets/Resources/Materials/Speckle Generated")) AssetDatabase.CreateFolder("Assets/Resources/Materials", "Speckle Generated");
          if (AssetDatabase.LoadAllAssetsAtPath("Assets/Resources/Materials/Speckle Generated/" + mat.name + ".mat").Length == 0)
            AssetDatabase.CreateAsset(mat, "Assets/Resources/Materials/Speckle Generated/" + mat.name + ".mat");
        }
        #endif


        return mat;
      }
      // 3. if not renderMaterial was passed, the default shader will be used 
      return mat;
    }

    #region ToSpeckle
    //TODO: more of these

    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public Point PointToSpeckle(Vector3 p) =>
      //switch y and z
      new Point(p.x, p.z, p.y);

    /// <summary>
    ///   Converts a Speckle mesh to a GameObject with a mesh renderer
    /// </summary>
    /// <param name="speckleMesh"></param>
    /// <returns></returns>
    #endregion

    #region ToNative
    #endregion

  }
}