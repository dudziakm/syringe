using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using Syringe.Core.Configuration;

namespace Syringe.Service.Api
{
	public class HealthCheckController : ApiController
	{
		private readonly IApplicationConfiguration _configuration;

		public HealthCheckController(IApplicationConfiguration configuration)
		{
			_configuration = configuration;
		}

		[Route("api/healthcheck/CheckConfiguration")]
		[HttpGet]
		public string CheckConfiguration()
		{
			if (string.IsNullOrEmpty(_configuration.WebsiteUrl))
				return "The service app.config WebsiteUrl key is empty - please enter the website url including port number in appSettings, e.g. http://localhost:1980";

			if (string.IsNullOrEmpty(_configuration.TestCasesBaseDirectory))
				return "The service app.config TestCasesBaseDirectory key is empty - please enter the folder the test case XML files are stored in appSettings, e.g. D:\\syringe";

			if (!Directory.Exists(_configuration.TestCasesBaseDirectory))
				return string.Format("The service app.config TestCasesBaseDirectory folder '{0}' does not exist", _configuration.TestCasesBaseDirectory);

			return "Everything is OK";
		}
	}
}