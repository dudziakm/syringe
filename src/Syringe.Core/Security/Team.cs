using System;
using System.Collections.Generic;
using Syringe.Core.TestCases.Configuration;

namespace Syringe.Core.Security
{
	public class Team
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public Config Config { get; set; }
		public List<Guid> UserIds { get; set; }

		public Team()
		{
			UserIds = new List<Guid>();
		}
	}
}
