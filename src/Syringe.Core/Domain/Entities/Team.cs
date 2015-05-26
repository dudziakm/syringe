using System;
using System.Collections.Generic;
using Syringe.Core.Configuration;

namespace Syringe.Core.Domain.Entities
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

		public override bool Equals(object obj)
		{
			Team other = obj as Team;
			return other != null && 
				   other.Id.Equals(Id) & other.Name.Equals(Name);
		}
	}
}
