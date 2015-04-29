using System;
using System.Collections.Generic;
using Syringe.Core.Http;

namespace Syringe.Tests.Unit.StubsMocks
{
	public class HttpClientMock : IHttpClient
	{
		public HttpResponse Response { get; set; }
		public List<TimeSpan> ResponseTimes { get; set; }
		private int _responseCounter;

		public HttpResponse ExecuteRequest(string httpMethod, string url, string contentType, string postBody, IEnumerable<KeyValuePair<string, string>> headers)
		{
			if (ResponseTimes != null)
			{
				Response.ResponseTime = ResponseTimes[_responseCounter++];
			}

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