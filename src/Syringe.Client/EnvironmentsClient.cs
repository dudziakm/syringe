using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using Syringe.Core.Services;
using Environment = Syringe.Core.Environment.Environment;

namespace Syringe.Client
{
	public class EnvironmentsClient : IEnvironmentsService
	{
		private readonly string _serviceUrl;

		public EnvironmentsClient(string serviceUrl)
		{
			_serviceUrl = serviceUrl;
		}

		public IEnumerable<Environment> List()
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("List");

			IRestResponse response = client.Execute(request);
			return DeserializeOrThrow<IEnumerable<Environment>>(response);
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
			return new RestRequest($"/api/environments/{action}");
		}
	}
}