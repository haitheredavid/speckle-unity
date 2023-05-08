using UnityEngine;

namespace Speckle.ConnectorUnity.Core.ScriptableConverter.Components
{
	public class SpeckleBrep : BaseBehaviour
	{
		public MeshFilter mesh
		{
			get => gameObject.GetComponent<MeshFilter>();
		}
	}
}