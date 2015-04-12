using System.Collections.Generic;
using System.IO;

namespace Syringe.Core
{
	public class Config
	{
		// Documented properties (that applicable to .NET)
		public string BaseUrl { get; set; }
		public string Proxy { get; set; }
		public string Useragent { get; set; }
		public string Httpauth { get; set; }
		public string GlobalHttpLog { get; set; }
		public string Comment { get; set; }
		public string Timeout { get; set; }
		public string GlobalTimeout { get; set; }

		// Custom variables, e.g. <baseurl3>
		public Dictionary<string, string> Variables { get; set; }

		public Config()
		{
			Variables = new Dictionary<string, string>();
		}
	}
}
