using System;
using Speckle.Core.Kits;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity
{

  public abstract class ComponentConverter : ScriptableObject, IComponentConverter
  {
    public bool storeProps = true;

    public abstract bool CanConvertToNative(Base type);
    public abstract bool CanConvertToSpeckle(Component type);

    public abstract GameObject ToNative(Base @base);
    public abstract Base ToSpeckle(Component component);
    public abstract string speckle_type { get; }

  }

  public abstract class ComponentConverter<TBase, TComponent> : ComponentConverter
    where TComponent : Component
    where TBase : Base
  {

    [SerializeField] private string typeName;

    protected virtual void OnEnable()
    {
      typeName = Activator.CreateInstance<TBase>().speckle_type;
    }

    // TODO: this is silly
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

        comp.properties = new SpeckleProperties
          { Data = @base.FetchProps() };
      }


      return root;
    }

    public override Base ToSpeckle(Component component)
    {
      return CanConvertToSpeckle(component) ? ConvertComponent((TComponent)component) : null;
    }

    public override string speckle_type
    {
      get => typeName;
    }

    protected bool IsRuntime
    {
      get => Application.isPlaying;
    }

    protected TComponent BuildGo(string goName = null)
    {
      return new GameObject(goName.Valid() ? goName : typeName).AddComponent<TComponent>();
    }
  }

}