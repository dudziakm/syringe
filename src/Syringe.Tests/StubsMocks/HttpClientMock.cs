using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Http;
using Syringe.Core.TestCases;

namespace Syringe.Tests.StubsMocks
{
	public class HttpClientMock : IHttpClient
	{
		private int _responseCounter;
		public HttpRequestInfo RequestInfo { get; set; }
		public List<TimeSpan> ResponseTimes { get; set; }

		public List<HttpRequestInfo> Responses { get; set; }

		public HttpClientMock(HttpRequestInfo requestInfo)
		{
			RequestInfo = requestInfo;
		}

		Task<HttpRequestInfo> IHttpClient.ExecuteRequestAsync(string httpMethod, string url, string contentType, string postBody, IEnumerable<HeaderItem> headers)
		{
			if (Responses == null)
			{
				if (ResponseTimes != null)
				{
					RequestInfo.ResponseTime = ResponseTimes[_responseCounter++];
				}

				return Task.FromResult(RequestInfo);
			}

			return Task.FromResult(Responses[_responseCounter++]);
		}
	}
}