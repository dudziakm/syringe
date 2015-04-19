using System.Collections.Generic;
using System.Net;

namespace Syringe.Core.Http.Logging
{
	public interface IHttpLogWriter
	{
		void AppendSeperator();
		void AppendRequest(string method, string url, IEnumerable<KeyValuePair<string, string>> headers);
		void AppendResponse(HttpStatusCode status, IEnumerable<KeyValuePair<string, string>> headers, string bodyResponse);
	}
}