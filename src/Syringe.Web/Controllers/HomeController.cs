﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Syringe.Core.Canary;
using Syringe.Core.Extensions;
using Syringe.Core.Repositories;
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
        private readonly Func<ICanaryService> _canaryClientFactory;

        public HomeController(
            ICaseService casesClient,
            IUserContext userContext,
            Func<IRunViewModel> runViewModelFactory,
            Func<ICanaryService> canaryClientFactory)
        {
            _casesClient = casesClient;
            _userContext = userContext;
            _runViewModelFactory = runViewModelFactory;
            _canaryClientFactory = canaryClientFactory;
        }

        public ActionResult Index(int pageNumber = 1, int noOfResults = 10)
        {
            CheckServiceIsRunning();

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

        private void CheckServiceIsRunning()
        {
            var canaryCheck = _canaryClientFactory();
            CanaryResult result = canaryCheck.Check();
            if (result == null || result.Success == false)
            {
                throw new InvalidOperationException("Unable to connect to the REST api service. Is the service started? Check it at http://localhost:8086/");
            }
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