using Syringe.Core.Http.Logging;

namespace Syringe.Tests.Unit
{
	public class HttpLogWriterStub : IHttpLogWriter
	{
		public void AppendSeperator()
		{
		}

		public void AppendRequest(RequestDetails requestDetails)
		{
		}

		public void AppendResponse(ResponseDetails responseDetails)
		{
		}

		public void AppendRequest(string method, string url, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> headers)
		{
		}

		public void AppendResponse(System.Net.HttpStatusCode status, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> headers, string bodyResponse)
		{
		}
	}
}