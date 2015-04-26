using System.Collections.Generic;
using System.Net;

namespace Syringe.Core.Http.Logging
{
	public class ResponseDetails
	{
		public HttpStatusCode Status { get; set; }
		public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
		public string BodyResponse { get; set; }
	}
}