using System.Web.Http;
using Syringe.Core.Canary;
using Syringe.Core.Services;

namespace Syringe.Service.Api
{
	public class CanaryController : ApiController, ICanaryService
	{
		[Route("api/canary/Check")]
		[HttpGet]
		public CanaryResult Check()
		{
			// TODO: Check Redis etc.
			return new CanaryResult() {Success = true, ErrorMessage = ""};
		}
	}
}