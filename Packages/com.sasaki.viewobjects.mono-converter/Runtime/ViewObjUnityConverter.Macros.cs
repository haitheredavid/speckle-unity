using UnityEngine;
using ViewTo.Connector.Unity;

namespace ViewTo.Objects.Converter.Unity
{
  public partial class ViewObjUnityConverter
  {

    private TViewObj Create<TViewObj>() where TViewObj : ViewObjBehaviour => NewObj.AddComponent<TViewObj>();

    private GameObject NewObj => new GameObject();

  }
}