using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using AD = UnityEditor.AssetDatabase;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter.Editor
{

    public static class ConnectorUnity
    {
        public static class Dirs
        {

            internal static string Folder => "Speckle";
            internal static string Main => "Assets";
            internal static string Converter => "Converter";
            internal static string Component => "Component";

            internal static string MainPath => $"{Main}";
            internal static string FolderPath => $"{MainPath}/{Folder}";
            internal static string ConverterPath => $"{FolderPath}/{Converter}";
            internal static string ComponentPath => $"{FolderPath}/{Component}";
        }

        [MenuItem("Speckle/Create Default Converters")]
        public static void CreateDefaultConverters()
        {
            if (!AD.IsValidFolder(Dirs.FolderPath)) AD.CreateFolder(Dirs.Main, Dirs.Folder);
            if (!AD.IsValidFolder(Dirs.ComponentPath)) AD.CreateFolder(Dirs.FolderPath, Dirs.Component);
            if (!AD.IsValidFolder(Dirs.ConverterPath)) AD.CreateFolder(Dirs.FolderPath, Dirs.Converter);

            BuildConverter<ConverterUnity>(Dirs.ConverterPath, Dirs.ComponentPath, true);

        }

        public static ScriptableConverter BuildConverter<TConverter>(string converterPath, string componentPath, bool save = false) where TConverter : ScriptableConverter
        {
            var converter = ScriptableObject.CreateInstance<TConverter>();

            AD.CreateAsset(converter, $"{converterPath}/{typeof(TConverter).GetLastName()}.asset");
            List<ComponentConverter> instances = new();

            foreach (var sc in converter.Converters)
            {
                var path = $"{componentPath}/{sc.GetLastName()}.asset";
                AD.CreateAsset(sc, path);
                var i = AD.LoadAssetAtPath<ComponentConverter>(path);
                instances.Add(i);
            }

            converter.Converters = instances;
            if (save)
            {
                EditorUtility.SetDirty(converter);
                AD.SaveAssetIfDirty(converter);
            }

            return converter;

        }


        internal static string GetLastName(this object t)
        {
            return t.GetType().ToString().Split('.').Last();
        }


    }



}
