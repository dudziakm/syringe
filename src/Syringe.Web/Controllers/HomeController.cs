using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Syringe.Core.Configuration;
using Syringe.Core.Extensions;
using Syringe.Core.Results;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	[Authorize]
    public class HomeController : Controller
    {
        private readonly ICaseService _casesClient;
        private readonly IUserContext _userContext;
        private readonly Func<IRunViewModel> _runViewModelFactory;
		private readonly IHealthCheck _healthCheck;

		public HomeController(
            ICaseService casesClient,
            IUserContext userContext,
            Func<IRunViewModel> runViewModelFactory,
			IHealthCheck healthCheck)
        {
            _casesClient = casesClient;
            _userContext = userContext;
            _runViewModelFactory = runViewModelFactory;
			_healthCheck = healthCheck;
        }

        public ActionResult Index(int pageNumber = 1, int noOfResults = 10)
        {
            RunHealthChecks();

			ViewBag.Title = "All test case files";

			// TODO: team name from the user context?
			IList<string> files = _casesClient.ListFilesForTeam(_userContext.TeamName).ToList();

            var model = new IndexViewModel
            {
                PageNumber = pageNumber,
                NoOfResults = noOfResults,
                PageNumbers = files.GetPageNumbersToShow(noOfResults),
                Files = files.GetPaged(noOfResults, pageNumber)
            };

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Run(string filename)
        {
			UserContext context = UserContext.GetFromFormsAuth(HttpContext);

			var runViewModel = _runViewModelFactory();
            runViewModel.Run(context, filename);
            return View("Run", runViewModel);
        }

        private void RunHealthChecks()
        {
			_healthCheck.CheckWebConfiguration();
			_healthCheck.CheckServiceConfiguration();
			_healthCheck.CheckServiceSwaggerIsRunning();
        }

        public ActionResult AllResults()
        {
            return View("AllResults", _casesClient.GetSummaries());
        }

        public ActionResult TodaysResults()
        {
            return View("AllResults", _casesClient.GetSummariesForToday());
        }

        public ActionResult ViewResult(Guid id)
        {
            return View("ViewResult", _casesClient.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteResult(Guid id)
        {
            TestCaseSession session = _casesClient.GetById(id);
            await _casesClient.DeleteAsync(session.Id);

            return RedirectToAction("AllResults");
        }
	}
}