using System.Collections.Generic;
using Speckle.Core.Api;

namespace Speckle.ConnectorUnity
{
	public static partial class OpsHelper
	{
		public static List<CommitWrapper> Wrap(this Commits commits)
		{
			var items = new List<CommitWrapper>();

			if (commits == null || !commits.items.Valid())
				return items;

			foreach (var commit in commits.items)
				items.Add(new CommitWrapper(commit));

			return items;
		}
	}
}