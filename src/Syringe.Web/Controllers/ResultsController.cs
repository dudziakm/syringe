using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Syringe.Core.Helpers;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.Tests.Results;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	[Authorize]
    public class ResultsController : Controller
    {
        private readonly ITasksService _tasksClient;
	    private readonly IUrlHelper _urlHelper;

	    public ResultsController(ITasksService tasksClient, IUrlHelper urlHelper)
	    {
	        _tasksClient = tasksClient;
	        _urlHelper = urlHelper;
	    }

	    public ActionResult Html(int taskId, int position)
        {
            TestResult testResult = FindTestResult(taskId, position);

            if (testResult == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Could not locate the specified test.");
            }

	        var baseUrl = _urlHelper.GetBaseUrl(testResult.ActualUrl);

            var viewModel = new ResultsViewModel
            {
                ActualUrl = testResult.ActualUrl,
                Content = testResult.HttpResponse == null ? string.Empty : _urlHelper.AddUrlBase(baseUrl, testResult.HttpResponse.Content)
            };

            return View(viewModel);
        }

        private TestResult FindTestResult(int taskId, int position)
        {
            TaskDetails taskDetails = _tasksClient.GetRunningTaskDetails(taskId);
            TestResult task = taskDetails.Results.ElementAtOrDefault(position);

            return task;
        }

        public ActionResult Raw(int taskId, int position)
        {
            TestResult testResult = FindTestResult(taskId, position);

            if (testResult == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Could not locate the specified test.");
            }

            var viewModel = new ResultsViewModel
            {
                ActualUrl = testResult.ActualUrl,
                Content = testResult.HttpResponse == null ? string.Empty : testResult.HttpResponse.Content
            };

            return View(viewModel);
        }
	}
}