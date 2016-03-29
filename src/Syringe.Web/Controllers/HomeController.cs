using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Syringe.Core.Configuration;
using Syringe.Core.Extensions;
using Syringe.Core.Helpers;
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
	    private readonly IUrlHelper _urlHelper;

	    public HomeController(
            ICaseService casesClient,
            IUserContext userContext,
            Func<IRunViewModel> runViewModelFactory,
			IHealthCheck healthCheck, 
            IUrlHelper urlHelper)
        {
            _casesClient = casesClient;
            _userContext = userContext;
            _runViewModelFactory = runViewModelFactory;
			_healthCheck = healthCheck;
	        _urlHelper = urlHelper;
        }

        public ActionResult  Position(int pageNumber = 1, int noOfResults = 10)
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

            return View(" Position", model);
        }

        [HttpPost]
        public ActionResult Run(string filename)
        {
			UserContext context = UserContext.GetFromFormsAuth(HttpContext);

			var runViewModel = _runViewModelFactory();
            runViewModel.Run(context, filename);
            return View("Run", runViewModel);
        }


        [HttpPost]
        public ActionResult RunTest(string filename, int index)
        {
            UserContext context = UserContext.GetFromFormsAuth(HttpContext);

            var runViewModel = _runViewModelFactory();
            runViewModel.RunTest(context, filename, index);
            return View("Run", runViewModel);
        }

        private void RunHealthChecks()
        {
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

		public ActionResult ViewHtml(Guid testCaseSessionId, Guid resultId)
		{
			TestCaseSession session = _casesClient.GetById(testCaseSessionId);
			TestCaseResult result = session.TestCaseResults.FirstOrDefault(x => x.Id == resultId);
			if (result != null)
			{
				string html = result.HttpContent;
				string baseUrl = _urlHelper.GetBaseUrl(result.ActualUrl);
				html = _urlHelper.AddUrlBase(baseUrl, html);

				return Content(html);
			}

			return Content("TestCaseResult Id not found");
		}

		public ActionResult ViewHttpLog(Guid testCaseSessionId, Guid resultId)
		{
			TestCaseSession session = _casesClient.GetById(testCaseSessionId);
			TestCaseResult result = session.TestCaseResults.FirstOrDefault(x => x.Id == resultId);
			if (result != null)
				return Content(result.HttpLog, "text/plain");

			return Content("TestCaseResult Id not found");
		}

		public ActionResult ViewLog(Guid testCaseSessionId, Guid resultId)
		{
			TestCaseSession session = _casesClient.GetById(testCaseSessionId);
			TestCaseResult result = session.TestCaseResults.FirstOrDefault(x => x.Id == resultId);
			if (result != null)
				return Content(result.Log, "text/plain");

			return Content("TestCaseResult Id not found");
		}
	}
}