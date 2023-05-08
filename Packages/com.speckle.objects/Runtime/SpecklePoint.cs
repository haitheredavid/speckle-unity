using UnityEngine;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter.Components
{
	// TODO: add in rendering support for speckle point
	public class SpecklePoint : BaseBehaviour
	{
		public Vector3 pos
		{
			get => transform.position;
			set => transform.position = pos;
		}
	}
}