using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity
{

	public abstract class ComponentConverter : ScriptableObject, IComponentConverter
	{

		public const string ModelUnits = Speckle.Core.Kits.Units.Meters;

		public bool storeProps = true;
		public bool convertProps = true;

		public abstract bool CanConvertToNative(Base type);
		public abstract bool CanConvertToSpeckle(Component type);

		public abstract GameObject ToNative(Base @base);
		public abstract Base ToSpeckle(Component component);

		public abstract string speckle_type { get; }
		public abstract string unity_type { get; }

		public abstract string targetType(bool toUnity);
	}

	public abstract class ComponentConverter<TBase, TComponent> : ComponentConverter
		where TComponent : Component
		where TBase : Base
	{

		[SerializeField, HideInInspector]
		private string unityTypeName;
		[SerializeField, HideInInspector]
		private string speckleTypeName;

		public override string speckle_type => speckleTypeName;

		public override string unity_type => unityTypeName;

		protected virtual HashSet<string> excludedProps
		{
			get
			{
				return new HashSet<string>(
					typeof(Base).GetProperties(
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase
					).Select(x => x.Name));
			}
		}

		protected bool IsRuntime
		{
			get => Application.isPlaying;
		}

		protected virtual void OnEnable()
		{
			speckleTypeName = Activator.CreateInstance<TBase>().speckle_type;
			unityTypeName = typeof(TComponent).ToString();
		}

		// TODO: this is silly, probably a much smarter way of handling this 
		public override bool CanConvertToNative(Base type)
		{
			return type != null && type.GetType() == typeof(TBase);
		}

		public override bool CanConvertToSpeckle(Component type)
		{
			return type != null && type.GetType() == typeof(TComponent);
		}

		protected abstract GameObject ConvertBase(TBase @base);

		protected abstract Base ConvertComponent(TComponent component);

		/// <summary>
		/// Unity Component to search for when trying to convert a game object
		/// </summary>
		public override GameObject ToNative(Base @base)
		{
			if (!CanConvertToNative(@base))
				return null;

			var root = ConvertBase((TBase)@base).gameObject;

			if (storeProps && root != null)
			{
				var comp = (BaseBehaviour)root.GetComponent(typeof(BaseBehaviour));
				if (comp == null)
					comp = root.AddComponent<BaseBehaviour>();

				// var props = @base.GetMembers().Where(x => !excludedProps.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
				comp.properties = new SpeckleProperties();
				comp.properties.Store(@base, excludedProps);
			}

			return root;
		}

		public override Base ToSpeckle(Component component)
		{
			return CanConvertToSpeckle(component) ? ConvertComponent((TComponent)component) : null;
		}

		/// <summary>
		/// helper function for getting the type associated with speckle or unity
		/// </summary>
		/// <param name="toUnity"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public override string targetType(bool toUnity)
		{
			return toUnity ? speckle_type : unity_type;
		}

		protected TComponent BuildGo(string goName = null)
		{
			return new GameObject(goName.Valid() ? goName : speckleTypeName).AddComponent<TComponent>();
		}
	}

}