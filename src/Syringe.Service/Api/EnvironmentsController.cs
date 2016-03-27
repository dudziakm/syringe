using System.Collections.Generic;
using System.Web.Http;
using Syringe.Core.Environment;

namespace Syringe.Service.Api
{
	public class EnvironmentsController : ApiController, IEnvironmentsService
	{
		private readonly IEnvironmentProvider _provider;

		public EnvironmentsController(IEnvironmentProvider provider)
		{
			_provider = provider;
		}

		[Route("api/environments/list")]
        [HttpGet]
        public IEnumerable<Environment> List()
        {
	        return _provider.GetAll();
        }
    }
}