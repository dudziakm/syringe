using System.Collections.Generic;

namespace Syringe.Core.TestCases.Configuration
{
	public class Config
	{
		// Documented properties (that are applicable to .NET)
		public string BaseUrl { get; set; }
		public string Proxy { get; set; }
		public string Useragent { get; set; }
		public string Httpauth { get; set; }
		public LogType GlobalHttpLog { get; set; }
		public string Comment { get; set; }
		public int Timeout { get; set; }
		public int GlobalTimeout { get; set; }

		// Custom variables, e.g. <baseurl3> - can contain duplicate keys
		public List<Variable> Variables { get; set; }

		public Config()
		{
			Variables = new List<Variable>();
		}
	}
}
