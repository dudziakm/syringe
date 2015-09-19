using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Syringe.Client;
using Syringe.Core.Canary;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;
using Syringe.Core.Security;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	public class HomeController : Controller
	{
	    private readonly CasesClient _casesClient;
		private readonly IUserContext _userContext;

		public HomeController()
		{
		    _casesClient = new CasesClient();
			_userContext = new UserContext();
		}

		public ActionResult Index()
		{
			CheckServiceIsRunning();

			// TODO: team name from the user context
			IEnumerable<string> files = _casesClient.ListFilesForTeam(_userContext.TeamName);

			var model = new IndexViewModel();
			model.AddFiles(files);

			return View(model);
		}

		public ActionResult Run(string filename)
		{
			return View("Run", "", filename);
		}

		private void CheckServiceIsRunning()
		{
			var canaryCheck = new CanaryClient();
			CanaryResult result = canaryCheck.Check();
			if (result == null || result.Success == false)
			{
				throw new InvalidOperationException("Unable to connect to the REST api service. Is the service started? Check it at http://localhost:22345/");
			}
		}

		public ActionResult AllResults()
		{
			var repository = new RavenDbTestCaseSessionRepository();
            return View("AllResults", repository.LoadAll());
		}

		public ActionResult ViewResult(Guid id)
		{
			var repository = new RavenDbTestCaseSessionRepository();
			return View("ViewResult", repository.GetById(id));
		}

		[HttpPost]
		public ActionResult DeleteResult(Guid id)
		{
			var repository = new RavenDbTestCaseSessionRepository();
			repository.Delete(id);

			return RedirectToAction("AllResults");
		}
	}
}