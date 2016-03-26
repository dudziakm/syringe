using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Syringe.Core.Helpers;
using Syringe.Core.Results;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
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

	    public ActionResult Html(int taskId, Guid caseId)
        {
            var taskCase = FindTestCaseResult(taskId, caseId);

            if (taskCase == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Could not locate the specified case.");
            }

	        var baseUrl = _urlHelper.GetBaseUrl(taskCase.ActualUrl);

            var viewModel = new ResultsViewModel
            {
                ActualUrl = taskCase.ActualUrl,
                Content = taskCase.HttpResponse == null ? string.Empty : _urlHelper.AddUrlBase(baseUrl, taskCase.HttpResponse.Content)
            };

            return View(viewModel);
        }

        private TestCaseResult FindTestCaseResult(int taskId, Guid caseId)
        {
            TaskDetails taskDetails = _tasksClient.GetRunningTaskDetails(taskId);

            TestCaseResult taskCase = taskDetails.Results.FirstOrDefault(x=>x.Id == caseId);
            return taskCase;
        }

        public ActionResult Raw(int taskId, Guid caseId)
        {
            var taskCase = FindTestCaseResult(taskId, caseId);

            if (taskCase == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Could not locate the specified case.");
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