using Newtonsoft.Json;
using RestSharp;
using Syringe.Core.Canary;
using Syringe.Core.Configuration;
using Syringe.Core.Services;

namespace Syringe.Client
{
	public class CanaryClient : ICanaryService
	{
		private readonly string _baseUrl;

		public CanaryClient()
			: this(new ApplicationConfig())
		{
		}

		public CanaryClient(IApplicationConfiguration appConfig)
		{
			_baseUrl = appConfig.ServiceUrl;
		}

		public CanaryResult Check()
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("Check");
			IRestResponse response = client.Execute(request);
			CanaryResult result = JsonConvert.DeserializeObject<CanaryResult>(response.Content);

			return result;
		}

		private IRestRequest CreateRequest(string action)
		{
			return new RestRequest(string.Format("/api/canary/{0}", action));
		}
	}
}