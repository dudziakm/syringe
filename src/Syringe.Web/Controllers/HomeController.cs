﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Syringe.Core.Canary;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
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
			var runViewModel = _runViewModelFactory();
			runViewModel.Run(_userContext, filename);
			return View("Run", runViewModel);
		}

		private void CheckServiceIsRunning()
		{
			var canaryCheck = _canaryClientFactory();
			CanaryResult result = canaryCheck.Check();
			if (result == null || result.Success == false)
			{
				throw new InvalidOperationException("Unable to connect to the REST api service. Is the service started? Check it at http://localhost:22345/");
			}
		}
	}
}