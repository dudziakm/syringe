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
		public HttpResponseInfo ResponseInfo { get; set; }
		public List<TimeSpan> ResponseTimes { get; set; }

		public List<HttpResponseInfo> Responses { get; set; }

		public HttpClientMock(HttpResponseInfo responseInfo)
		{
			ResponseInfo = responseInfo;
		}

		Task<HttpResponseInfo> IHttpClient.ExecuteRequestAsync(string httpMethod, string url, string contentType, string postBody, IEnumerable<HeaderItem> headers)
		{
			if (Responses == null)
			{
				if (ResponseTimes != null)
				{
					ResponseInfo.ResponseTime = ResponseTimes[_responseCounter++];
				}

				return Task.FromResult(ResponseInfo);
			}

			return Task.FromResult(Responses[_responseCounter++]);
		}
	}
}