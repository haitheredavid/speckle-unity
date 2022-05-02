﻿using System;
using System.Collections.Generic;
using Speckle.Core.Api;

namespace Speckle.ConnectorUnity
{
	[Serializable]
	public class BranchWrapper
	{

		public readonly List<CommitWrapper> commits;
		public readonly string description;

		public readonly string id;
		public readonly string name;
		public BranchWrapper(Branch branch)
		{
			id = branch.id;
			name = branch.name;
			description = branch.description;

			commits = branch.commits.Wrap();
		}
	}
}