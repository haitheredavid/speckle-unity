using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter
{

    public abstract class ComponentConverter : ScriptableObject, IComponentConverter
    {
        /// <summary>
        /// Simple data container of what the component supports
        /// </summary>
        [SerializeField] protected ComponentInfo info;

        public ScriptableConverter parent {get;set;}

        /// <summary>
        /// A container for managing the conversion settings
        /// </summary>
        public ConverterSettings settings {get;set;}

        /// <summary>
        /// The unity <see cref="Component"/> type targeted for conversion
        /// </summary>
        public string unityType => info.unityTypeName;

        /// <summary>
        /// The <see cref="Base"/> speckle type targeted for conversion
        /// </summary>
        public string speckleType => info.speckleTypeName;

        public virtual bool isInit => parent != null;

        public abstract void Initialize();

        /// <summary>
        /// <para> Typical speckle conversion called from <seealso cref="ISpeckleConverter"/>
        /// This method is mainly used for setting up the gameobject and component data on the  main thread</para>
        ///
        /// <para>The conversions happen in two styles, one by one on the main thread when using <see cref="ConverterSettings.ConversionStyle.Sync"/>
        /// or they can be in different threads when using <see cref="ConverterSettings.ConversionStyle.Queue"/>.
        /// Conversion style can be changed in the <see cref="settings"/> object</para>
        /// 
        /// </summary>
        /// <param name="base">The <see cref="Base"/> object to convert</param>
        /// <returns>The scene object with necessary component info</returns>
        public abstract GameObject ToNative(Base @base);

        /// <summary>
        /// Simple check to see if <see cref="Base"/> is supported by the converter
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Returns true if casted type is supported</returns>
        public abstract bool CanConvertToNative(Base type);

        /// <summary>
        /// Simple check to see if <see cref="Component"/> is supported by the converter
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Returns true if casted type is supported</returns>
        public abstract bool CanConvertToSpeckle(Component type);

        /// <summary>
        /// Conversion logic to parse the unity components into a Speckle Object
        /// </summary>
        /// <param name="component">Component to convert</param>
        /// <returns>The <see cref="Base"/> processed</returns>
        public abstract Base ToSpeckle(Component component);

        /// <summary>
        /// Returns true if <see cref="unityType"/> and <see cref="speckleType"/> are same
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ComponentConverter other)
        {
            return other != null &&
                   other.unityType != null &&
                   other.unityType == unityType &&
                   other.speckleType.Valid() &&
                   other.speckleType.Equals(speckleType);
        }

        [Serializable]
        public struct ComponentInfo
        {
            public string speckleTypeName;

            public string unityTypeName;

            public ComponentInfo(string unity, string speckle)
            {
                unityTypeName = unity;
                speckleTypeName = speckle;
            }
        }

    }

    public abstract class ComponentConverter<TBase, TComponent> : ComponentConverter, IHaveAQueue
        where TComponent : Component
        where TBase : Base
    {

        [SerializeField] protected TComponent prefab;

        [SerializeField]
        [HideInInspector]
        protected ConverterObjectBuilder builder;

        /// <summary>
        /// Checks if any items are being converted still
        /// </summary>
        public bool isWorking => builder != null && builder.isWorking;

        /// <summary>
        /// checks if the parent and builder are ready to go
        /// </summary>
        public override bool isInit => base.isInit && builder != null && builder.isInit;

        public event Action<int> OnQueueSizeChanged;

        public override void Initialize()
        {
            if (builder == null) builder = new GameObject().AddComponent<ConverterObjectBuilder>();

            builder.Initialize(data => Serialize((TBase)data.speckleObj, (TComponent)data.unityObj));
            builder.OnQueueSizeChange += this.OnQueueSizeChanged;

        }

        /// <inheritdoc />
        public override bool CanConvertToNative(Base type)
        {
            return type != null && type.GetType() == typeof(TBase);
        }

        /// <inheritdoc />
        public override bool CanConvertToSpeckle(Component type)
        {
            return type != null && type.GetType() == typeof(TComponent);
        }

        /// <inheritdoc />
        public override Base ToSpeckle(Component component)
        {

            return CanConvertToSpeckle(component) ? Deserialize((TComponent)component) : null;
        }


        public override GameObject ToNative(Base @base)
        {
            var comp = CreateUnityInstance();

            if (@base is not TBase obj)
            {
                Debug.Log($"Speckle object mismatch: {@base.speckle_type}\nWas expecting: {info.speckleTypeName}");
                return comp.gameObject;
            }

            builder.AddToQueue(new BuilderDataInput(obj, comp));

            return comp.gameObject;
        }


        /// <summary>
        /// Creates a <see cref="Component"/> with the type component attached to it
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected virtual TComponent CreateUnityInstance(Transform parent = null)
        {
            var obj = Instantiate(prefab, parent);
            obj.name = speckleType;
            return obj;
        }


        protected virtual TBase CreateSpeckleInstance()
        {
            return Activator.CreateInstance<TBase>();
        }


        /// <summary>
        /// Nested method from <see cref="ToSpeckle(UnityEngine.Component)"/> that sets the types for conversion
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target"></param>
        protected abstract void Serialize(TBase obj, TComponent target);

        protected abstract TBase Deserialize(TComponent obj);


        protected virtual void OnEnable()
        {
            info = new ComponentInfo(typeof(TComponent).ToString(), Activator.CreateInstance<TBase>().speckle_type);
        }

        protected void OnDisable()
        { }


    }

}
