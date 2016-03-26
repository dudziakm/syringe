using System;
using System.Globalization;
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

            TestCaseResult taskCase = (from d in taskDetails.Results where d.TestCase.Id == caseId select d).FirstOrDefault();
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

		// TODO: put these in their own place, some kind of HTML cleaner class.
		public static string GetBaseUrl(string url)
		{
			Uri result;
			if (Uri.TryCreate(url, UriKind.Absolute, out result))
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}://{1}/", result.Scheme, result.Host);
			}

			return url;
		}

		public static string AddUrlBase(string baseUrl, string content)
		{
			// add base tag to href
			var htmlUpdated = content.Replace("href=\"/", "href=\"" + baseUrl);
			// add base tag to src 
			htmlUpdated = htmlUpdated.Replace("src=\"/", "src=\"" + baseUrl);

			return htmlUpdated;
		}
	}
}