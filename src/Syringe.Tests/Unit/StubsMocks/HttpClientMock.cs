using System.Collections.Generic;
using Syringe.Core.Http;

namespace Syringe.Tests.Unit.StubsMocks
{
	public class HttpClientMock : IHttpClient
	{
		public HttpResponse Response { get; set; }

		public HttpResponse ExecuteRequest(string httpMethod, string url, string contentType, string postBody, IEnumerable<KeyValuePair<string, string>> headers)
		{
			return Response;
		}

		public void LogLastRequest()
		{
		}

		public void LogLastResponse()
		{
		}
	}
}