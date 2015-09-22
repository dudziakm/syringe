using System.Collections.Generic;
using System.Net;
using Syringe.Core.Http.Logging;

namespace Syringe.Tests.StubsMocks
{
	public class HttpLogWriterMock : IHttpLogWriter
	{
		public RequestDetails RequestDetails { get; set; }
		public ResponseDetails ResponseDetails { get; set; }

		public void AppendSeperator()
		{
		}

		public void AppendRequest(RequestDetails requestDetails)
		{
			RequestDetails = requestDetails;
		}

		public void AppendResponse(ResponseDetails responseDetails)
		{
			ResponseDetails = responseDetails;
		}

		public void AppendRequest(string method, string url, IEnumerable<KeyValuePair<string, string>> headers)
		{
		}

		public void AppendResponse(HttpStatusCode status, IEnumerable<KeyValuePair<string, string>> headers, string bodyResponse)
		{
		}
	}
}