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

	    public ActionResult Html(int taskId, Guid id)
        {
            var taskCase = FindTestCaseResult(taskId, id);

            if (taskCase == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Could not locate the specified test.");
            }

	        var baseUrl = _urlHelper.GetBaseUrl(taskCase.ActualUrl);

            var viewModel = new ResultsViewModel
            {
                ActualUrl = taskCase.ActualUrl,
                Content = taskCase.HttpResponse == null ? string.Empty : _urlHelper.AddUrlBase(baseUrl, taskCase.HttpResponse.Content)
            };

            return View(viewModel);
        }

        private TestResult FindTestCaseResult(int taskId, Guid id)
        {
            TaskDetails taskDetails = _tasksClient.GetRunningTaskDetails(taskId);

            TestResult task = taskDetails.Results.First(x => x.Id == id);
            return task;
        }

        public ActionResult Raw(int taskId, Guid id)
        {
            var taskCase = FindTestCaseResult(taskId, id);

            if (taskCase == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Could not locate the specified test.");
            }

            var viewModel = new ResultsViewModel
            {
                ActualUrl = taskCase.ActualUrl,
                Content = taskCase.HttpResponse == null ? string.Empty : taskCase.HttpResponse.Content
            };

            return View(viewModel);
        }

	}
}