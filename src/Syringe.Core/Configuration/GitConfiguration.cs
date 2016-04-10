using System.Collections.Generic;

namespace Syringe.Core.Configuration
{
	public class GitConfiguration
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string RepositoryUrl { get; set; }
		public List<string> Branches { get; set; }

		public GitConfiguration()
		{
			RepositoryUrl = "";
			Branches = new List<string>() { "master" };
		}
	}
}