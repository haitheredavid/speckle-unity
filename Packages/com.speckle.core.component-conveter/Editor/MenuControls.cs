using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Speckle.ConnectorUnity.Converter.Editor
{

    public class MenuControls
    {

        [MenuItem("Speckle/Create Default Converters")]
        public static void CreateDefaultConverters()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Speckle"))
            {
                AssetDatabase.CreateFolder("Assets", "Speckle");
            }

            if (!AssetDatabase.IsValidFolder("Assets/Speckle/Converter"))
            {
                AssetDatabase.CreateFolder("Assets/Speckle", "Converter");
            }

            var converter = ScriptableObject.CreateInstance<ConverterUnity>();
            AssetDatabase.CreateAsset(converter, "Assets/Speckle/Converter/Converter-Unity.asset");

            AssetDatabase.CreateFolder("Assets/Speckle", "Component");
            converter.Converters.ForEach(x => AssetDatabase.CreateAsset(x,$"Assets/Speckle/Component/{x.GetType().ToString().Split('.').Last()}.asset") );
        }


    }



}
