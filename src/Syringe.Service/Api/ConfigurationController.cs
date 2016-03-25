using System.Web.Http;
using Syringe.Client;
using Syringe.Core.Configuration;

namespace Syringe.Service.Api
{
	public class ConfigurationController : ApiController, IConfigurationService
	{
		private readonly IConfiguration _configuration;

		public ConfigurationController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[Route("api/configuration/")]
		[HttpGet]
		public IConfiguration GetConfiguration()
		{
			return _configuration;
		}
	}
}
