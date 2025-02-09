﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Speckle.Core.Kits;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using UnityEngine;

[assembly: InternalsVisibleTo(assemblyName: "Speckle.ConnectorUnity.Core.ScriptableConverter.Editor")]

namespace Speckle.ConnectorUnity.Core.ScriptableConverter
{

    /// <summary>
    /// A unity friendly version of <see cref="ISpeckleConverter"/> that utilizes <see cref="ScriptableObject"/> for modular converters
    /// </summary>
    public abstract class ScriptableConverter : ScriptableObject, ISpeckleConverter
    {

        [SerializeField] ConverterSettings settings;

        /// <summary>
        /// Returns the name of the assigned to the <see cref="ScriptableObject.name"/> object in the editor 
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Simple description of the converter
        /// </summary>
        [field: SerializeField]
        public string Description {get;private set;}

        /// <summary>
        /// The human responsible
        /// </summary>
        [field: SerializeField]
        public string Author {get;private set;}

        /// <summary>
        /// Some sort of context info
        /// </summary>
        [field: SerializeField]
        public string WebsiteOrEmail {get;private set;}

        /// <summary>
        /// How an object is handled when the data is received in the application. Default is set to Create
        /// </summary>
        [field: SerializeField]
        public ReceiveMode ReceiveMode {get;set;} = ReceiveMode.Create;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public List<ComponentConverter> Converters {get;internal set;} = new();

        /// <summary>
        /// 
        /// </summary>
        public HashSet<Exception> ConversionErrors {get;protected set;} = new();

        /// <summary>
        /// 
        /// </summary>
        public List<ApplicationObject> ContextObjects {get;set;} = new();

        /// <summary>
        /// An object for capturing the status of speckle operations
        /// </summary>
        public ProgressReport Report {get;protected set;} = new();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetServicedApplications()
        {
            return new[] {HostApplications.Unity.Name};
        }

        /// <summary>
        /// A backup method for assigning <see cref="Converters"/> if none were set in the editor 
        /// </summary>
        /// <returns></returns>
        protected abstract List<ComponentConverter> GetDefaultConverters();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        public virtual void SetContextObjects(List<ApplicationObject> objects)
        {
            ContextObjects = objects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        public virtual void SetPreviousContextObjects(List<ApplicationObject> objects)
        {
            ContextObjects = objects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual void SetContextDocument(object doc)
        {
            if (doc is not Dictionary<string, UnityEngine.Object> loadedAssets)
                throw new ArgumentException($"Expected {nameof(doc)} to be of type {typeof(Dictionary<string, UnityEngine.Object>)}", nameof(doc));

            // LoadedAssets = loadedAssets;
        }

        /// <summary>
        /// Pass in a simple settings object. The <param name="settings"></param> expects type <seealso cref="ConverterSettings"/> 
        /// </summary>
        /// <param name="settings"></param>
        public virtual void SetConverterSettings(object settings)
        {
            if (settings is ConverterSettings converterSettings) this.settings = converterSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public virtual Base ConvertToSpeckle(object @object)
        {
            return TryGetConverter(@object, true, out var comp, out var converter) ? converter.ToSpeckle(comp) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public virtual List<Base> ConvertToSpeckle(List<object> objects)
        {
            return objects.Valid() ? objects.Select(ConvertToSpeckle).ToList() : new List<Base>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base"></param>
        /// <returns></returns>
        public virtual object ConvertToNative(Base @base)
        {
            return TryGetConverter(@base, true, out var converter) ? converter.ToNative(@base) : null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public virtual List<object> ConvertToNative(List<Base> objects)
        {
            return objects.Valid() ? objects.Select(ConvertToNative).ToList() : new List<object>();
        }

        /// <summary>
        /// Checks if any of the <see cref="Converters"/> support the <paramref name="object"/> passed in 
        /// </summary>
        /// <param name="object"></param>
        /// <returns>Returns true if a converter is found for the type</returns>
        public virtual bool CanConvertToSpeckle(object @object)
        {
            return TryGetConverter(@object, false, out _, out _);
        }

        /// <summary>
        /// Checks if any of the <see cref="Converters"/> support the <paramref name="base"/> passed in 
        /// </summary>
        /// <param name="base"></param>
        /// <returns>Returns true if a converter is found for the type</returns>
        public virtual bool CanConvertToNative(Base @base)
        {
            return TryGetConverter(@base, false, out _);
        }

        protected virtual void OnEnable()
        {
            Report = new();
            ConversionErrors = new();

            if (!Converters.Valid()) Converters = GetDefaultConverters();

            if (settings == null)
            {
                SetConverterSettings(new ConverterSettings() {style = ConverterSettings.ConversionStyle.Queue});
            }
        }


        //TODO: maybe store these in a better way? 
        bool TryGetConverter(Base obj, bool init, out ComponentConverter converter)
        {
            converter = null;

            if (!Converters.Valid())
            {
                SpeckleUnity.Console.Log($"No valid Converters found in {name}");
                return false;
            }

            if (obj == null)
            {
                SpeckleUnity.Console.Log($"Object is null, please check the source of the object you are trying to pass into {name}");
                return false;
            }

            foreach (var c in Converters)
            {
                if (c == null)
                    continue;

                if (c.speckleType.Equals(obj.speckle_type))
                {
                    converter = c;

                    if (init && !c.isInit)
                    {
                        c.parent = this;
                        c.settings = settings;
                        c.Initialize();
                    }
                    break;
                }
            }

            return converter != null;
        }

        bool TryGetConverter(object obj, bool init, out Component comp, out IComponentConverter converter)
        {
            comp = null;
            converter = default(IComponentConverter);

            if (!Converters.Valid())
            {
                SpeckleUnity.Console.Log($"No valid Converters found in {name}");
                return false;
            }

            if (obj == null)
            {
                SpeckleUnity.Console.Log($"Object is null, please check the source of the object you are trying to pass into {name}");
                return false;
            }

            switch (obj)
            {
                case GameObject o:
                    foreach (var c in Converters)
                    {
                        if (c == null || o.GetComponent(c.unityType))
                            continue;

                        converter = c;

                        if (init)
                            c.settings = settings;

                        break;
                    }

                    break;

                case Component o:
                    comp = o;
                    foreach (var c in Converters)
                    {
                        if (c == null || c.unityType != comp.GetType().ToString())
                            continue;

                        converter = c;
                        break;
                    }

                    break;
                default:
                    Debug.LogException(new SpeckleException($"Native unity object {obj.GetType()} is not supported"));
                    break;
            }

            return converter != default(object) && comp != null;
        }


    }

}
