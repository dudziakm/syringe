using System;
using System.Collections.Generic;
using Syringe.Core;
using Syringe.Core.Http;

namespace Syringe.Tests.Unit.StubsMocks
{
	public class HttpClientMock : IHttpClient
	{
		private int _responseCounter;
		public bool LogLastRequestCalled { get; set; }
		public bool LogLastResponseCalled { get; set; }
		public HttpResponse Response { get; set; }
		public List<TimeSpan> ResponseTimes { get; set; }

		public List<HttpResponse> Responses { get; set; }

		public HttpClientMock(HttpResponse response)
		{
			Response = response;
		}

		public HttpResponse ExecuteRequest(string httpMethod, string url, string contentType, string postBody, IEnumerable<HeaderItem> headers)
		{
			if (Responses == null)
			{
				if (ResponseTimes != null)
				{
					Response.ResponseTime = ResponseTimes[_responseCounter++];
				}

				return Response;
			}
			else
			{
				return Responses[_responseCounter++];
			}
		}

		public void LogLastRequest()
		{
			LogLastRequestCalled = true;
		}

		public void LogLastResponse()
		{
			LogLastResponseCalled = true;
		}
	}
}