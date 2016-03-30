using System.Collections.Generic;
using Syringe.Core.Tests;

namespace Syringe.Core.Http.Logging
{
	public class RequestDetails
	{
		public string Method { get; set; }
		public string Url { get; set; }
		public IEnumerable<HeaderItem> Headers { get; set; }
		public string Body { get; set; }
	}
}