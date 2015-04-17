using System;
using System.Collections.Generic;
using System.Net;

namespace Syringe.Core.Http
{
	public class HttpResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public string Content { get; set; }
		public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
	    public TimeSpan ResponseTime { get; set; }
	}
}