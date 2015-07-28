using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using Syringe.Core;
using Syringe.Core.Configuration;
using Syringe.Core.Domain.Entities;
using Syringe.Core.Domain.Services;
using Syringe.Core.Xml;

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