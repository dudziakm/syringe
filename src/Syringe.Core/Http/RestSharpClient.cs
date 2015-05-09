using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;
using Syringe.Core.Extensions;
using Syringe.Core.Http.Logging;

namespace Syringe.Core.Http
{
	public class RestSharpClient : IHttpClient
	{
		private readonly IHttpLogWriter _httpLogWriter;
		private readonly CookieContainer _cookieContainer;
		private RequestDetails _lastRequest;
		private ResponseDetails _lastResponse;

		public RestSharpClient(IHttpLogWriter httpLogWriter)
		{
			_httpLogWriter = httpLogWriter;
			_cookieContainer = new CookieContainer();
		}

		public HttpResponse ExecuteRequest(string httpMethod, string url, string contentType, string postBody, IEnumerable<KeyValuePair<string, string>> headers)
		{
			var client = new RestClient(url);
			client.CookieContainer = _cookieContainer;

			//
			// Make the request adding the content-type, body and headers
			//
			Method method = GetMethodEnum(httpMethod);
			var request = new RestRequest(method);
			if (method == Method.POST)
			{
				request.AddParameter(contentType, postBody, ParameterType.RequestBody);
			}

			headers = headers.ToList();
			foreach (var keyValuePair in headers)
			{
				request.AddHeader(keyValuePair.Key, keyValuePair.Value);
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
			IRestResponse response = client.Execute(request);
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
