using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;
using Syringe.Core.Extensions;
using Syringe.Core.Http.Logging;

namespace Syringe.Core.Http
{
	public class HttpClient : IHttpClient
	{
		private readonly IHttpLogWriter _httpLogWriter;
		private readonly IRestClient _restClient;
		private readonly CookieContainer _cookieContainer;
		private RequestDetails _lastRequest;
		private ResponseDetails _lastResponse;

		public HttpClient(IHttpLogWriter httpLogWriter, IRestClient restClient)
		{
			_httpLogWriter = httpLogWriter;
			_restClient = restClient;
			_cookieContainer = new CookieContainer();
		}

		public HttpResponse ExecuteRequest(string httpMethod, string url, string contentType, string postBody, IEnumerable<KeyValuePair<string, string>> headers)
		{
			Uri uri;
			if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
				throw new ArgumentException(url + " is not a valid Uri", "url");

			_restClient.BaseUrl = uri;
			_restClient.CookieContainer = _cookieContainer;

			//
			// Make the request adding the content-type, body and headers
			//
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

			_lastRequest = new RequestDetails()
			{
				Body = postBody,
				Headers = headers,
				Method = httpMethod,
				Url = url
			};

			//
			// Get the response back, parsing the headers
			//
            DateTime startTime = DateTime.UtcNow;
			IRestResponse response = _restClient.Execute(request);
		    TimeSpan responseTime = DateTime.UtcNow - startTime;

			List<KeyValuePair<string, string>> keyvaluePairs = new List<KeyValuePair<string, string>>();
			if (response.Headers != null)
			{ 
				keyvaluePairs = response.Headers.Select(x => new KeyValuePair<string, string>(x.Name, Convert.ToString(x.Value)))
												.ToList();
			}

			_lastResponse = new ResponseDetails()
			{
				BodyResponse = response.Content,
				Headers = keyvaluePairs,
				Status = response.StatusCode
			};

			return new HttpResponse()
			{
				StatusCode = response.StatusCode,
				Content = response.Content,
				Headers = keyvaluePairs,
                ResponseTime = responseTime
			};
		}

		public void LogLastRequest()
		{
			_httpLogWriter.AppendRequest(_lastRequest);			
		}

		public void LogLastResponse()
		{
			_httpLogWriter.AppendResponse(_lastResponse);
			_httpLogWriter.AppendSeperator();
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
