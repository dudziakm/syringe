using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core.TestCases;

namespace Syringe.Core.Http
{
	public class HttpClient : IHttpClient
	{
		private readonly IRestClient _restClient;
		private readonly CookieContainer _cookieContainer;

		public HttpClient(IRestClient restClient)
		{
			_restClient = restClient;
			_cookieContainer = new CookieContainer();
		}

		public async Task<HttpResponseInfo> ExecuteRequestAsync(string httpMethod, string url, string contentType, string postBody, IEnumerable<HeaderItem> headers)
		{
			Uri uri;
			if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
				throw new ArgumentException(url + " is not a valid Uri", "url");

			_restClient.BaseUrl = uri;
			_restClient.CookieContainer = _cookieContainer;

			// Make the request adding the content-type, body and headers
			Method method = GetMethodEnum(httpMethod);
			var request = new RestRequest(method);
			if (method == Method.POST)
			{
				// From the RestSharp docs:
				// "The name of the parameter will be used as the Content-Type header for the request."
				request.AddParameter(contentType, postBody, ParameterType.RequestBody);
			}

			if (headers != null)
			{
				headers = headers.ToList();
				foreach (var keyValuePair in headers)
				{
					request.AddHeader(keyValuePair.Key, keyValuePair.Value);
				}
			}

			// Get the response back
            DateTime startTime = DateTime.UtcNow;
			IRestResponse response = await _restClient.ExecuteTaskAsync(request);
		    TimeSpan responseTime = DateTime.UtcNow - startTime;

			return new HttpResponseInfo()
			{
				Response = response,
				ResponseTime = responseTime
			};
		}

		private Method GetMethodEnum(string httpMethod)
		{
			var method = Method.GET;

			if (!Enum.TryParse(httpMethod, true, out method))
			{
				method = Method.GET;
			}

			return method;
		}
	}
}
