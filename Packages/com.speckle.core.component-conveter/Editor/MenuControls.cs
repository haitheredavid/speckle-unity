using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using AD= UnityEditor.AssetDatabase;

namespace Speckle.ConnectorUnity.Converter.Editor
{

    public static class MenuControls
    {
        internal class Dirs
        {

            internal static string Folder => "Speckle";
            internal static string Main => "Assets";
            internal static string Converter => "Converter";
            internal static string Component => "Component";

            internal static string MainPath => $"{Main}/";
            internal static string FolderPath => $"{MainPath}{Folder}/";
            internal static string ConverterPath => $"{FolderPath}{Converter}/";
            internal static string ComponentPath => $"{FolderPath}{Component}/";
        }

        [MenuItem("Speckle/Create Default Converters")]
        public static void CreateDefaultConverters()
        {
            if (!AD.IsValidFolder(Dirs.FolderPath)) AD.CreateFolder(Dirs.Main, Dirs.Folder);
            if (!AD.IsValidFolder(Dirs.ComponentPath)) AD.CreateFolder(Dirs.FolderPath, Dirs.Component);
            if (!AD.IsValidFolder(Dirs.ConverterPath)) AD.CreateFolder(Dirs.FolderPath, Dirs.Converter);

            var converter = ScriptableObject.CreateInstance<ConverterUnity>();
            AD.CreateAsset(converter, $"{Dirs.ConverterPath}{converter.GetLastName()}.asset");
            
            converter.Converters.ForEach(x => AD.CreateAsset(x, $"{Dirs.ComponentPath}{x.GetLastName()}.asset"));
        }

        internal static string GetLastName(this object t)
        {
            return t.GetType().ToString().Split('.').Last();
        }


    }



}
