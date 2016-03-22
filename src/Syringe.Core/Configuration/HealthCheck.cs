using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core.Exceptions;

namespace Syringe.Core.Configuration
{
	public class HealthCheck : IHealthCheck
	{
		private readonly IApplicationConfiguration _configuration;

		public HealthCheck(IApplicationConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void CheckWebConfiguration()
		{
			if (string.IsNullOrEmpty(_configuration.ServiceUrl))
				throw new HealthCheckException("The web.config ServiceUrl key is empty in the web.config - please enter the service url including port number in appSettings, e.g. http://localhost:1981");
		}

		public void CheckServiceConfiguration()
		{
			var client = new RestClient(_configuration.ServiceUrl);
			var request = new RestRequest("/api/healthcheck/CheckConfiguration");

			IRestResponse response = client.Execute(request);

			if (response.StatusCode != HttpStatusCode.OK)
				throw new HealthCheckException("The REST service at {0} did not return a 200 OK. Is the service running?", response.ResponseUri);

			if (!response.Content.Contains("Everything is OK"))
				throw new HealthCheckException("The REST service at {0} configuration check failed: \n{1}", response.ResponseUri, response.Content);
		}

		public void CheckServiceSwaggerIsRunning()
		{
			var client = new RestClient(_configuration.ServiceUrl);
			var request = new RestRequest("/swagger/ui/index");

			IRestResponse response = client.Execute(request);

			if (response.Content.Contains("Syringe REST API"))
				throw new HealthCheckException("The REST service at {0} did not return content with 'Syringe REST API' in the body.", response.ResponseUri);
		}
	}
}
