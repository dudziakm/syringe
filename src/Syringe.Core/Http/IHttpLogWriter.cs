using System.Collections.Generic;
using System.Net;

namespace Syringe.Core.Http
{
	public interface IHttpLogWriter
	{
		void WriteSeperator();
		void WriteRequest(string method, string url, IEnumerable<KeyValuePair<string, string>> headers);
		void WriteResponse(HttpStatusCode status, IEnumerable<KeyValuePair<string, string>> headers, string bodyResponse);
	}
}