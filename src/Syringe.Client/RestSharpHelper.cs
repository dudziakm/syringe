using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace Syringe.Client
{
	public class RestSharpHelper
	{
		private readonly string _requestPath;

		public RestSharpHelper(string requestPath)
		{
			_requestPath = requestPath;
		}

		public T DeserializeOrThrow<T>(IRestResponse response)
		{
			if (response.StatusCode == HttpStatusCode.OK)
			{
				return JsonConvert.DeserializeObject<T>(response.Content);
			}

			throw new ClientException("{0} - {1}", response.StatusCode, response.Content);
		}

		public IRestRequest CreateRequest(string action)
		{
			return new RestRequest($"{_requestPath}/{action}");
		}
	}
}