using System;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using Syringe.Core.Configuration;

namespace Syringe.Client
{
	public class ConfigurationClient : IConfigurationService
	{
		private readonly string _serviceUrl;

		public ConfigurationClient(string serviceUrl)
		{
			_serviceUrl = serviceUrl;
		}

		public IConfiguration GetConfiguration()
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("");

			IRestResponse response = client.Execute(request);
			return DeserializeOrThrow<JsonConfiguration>(response);
		}

	    private T DeserializeOrThrow<T>(IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
			{
				return JsonConvert.DeserializeObject<T>(response.Content);
			}

            throw new Exception(response.Content);
        }

	    private IRestRequest CreateRequest(string action)
		{
			return new RestRequest($"/api/configuration/{action}");
		}
	}
}