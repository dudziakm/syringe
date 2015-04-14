using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using RestSharp;

namespace Syringe.Core.Http
{
	public class RestSharpClient : IHttpClient
	{
		private readonly CookieContainer _cookieContainer;

		public RestSharpClient()
		{
			_cookieContainer = new CookieContainer();
		}

		public HttpResponse MakeRequest(string httpMethod, string url, string contentType, string postBody, IEnumerable<KeyValuePair<string, string>> headers)
		{
			var client = new RestClient(url);
			client.CookieContainer = _cookieContainer;

			Method method = GetMethodEnum(httpMethod);
			var request = new RestRequest(method);
			if (method == Method.POST)
			{
				request.AddParameter(contentType, postBody, ParameterType.RequestBody);
			}

			foreach (var keyValuePair in headers)
			{
				request.AddHeader(keyValuePair.Key, keyValuePair.Value);
			}

			IRestResponse response = client.Execute(request);
			List<KeyValuePair<string, string>> keyvaluePairs = new List<KeyValuePair<string, string>>();
			if (response.Headers != null)
			{ 
				keyvaluePairs = response.Headers.Select(x => new KeyValuePair<string, string>(x.Name, Convert.ToString(x.Value)))
												.ToList();
			}

			return new HttpResponse()
			{
				StatusCode = response.StatusCode,
				Content = response.Content,
				Headers = keyvaluePairs
			};
		}

		private Method GetMethodEnum(string httpMethod)
		{
			var method = Method.GET;

			if (Enum.IsDefined(typeof(Method), httpMethod))
			{
				Enum.TryParse(httpMethod, out method);
			}

			return method;
		}
	}
}
