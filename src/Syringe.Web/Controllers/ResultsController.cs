using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HtmlAgilityPack;
using Syringe.Core.Results;
using Syringe.Core.Services;
using Syringe.Core.Tasks;

namespace Syringe.Web.Controllers
{
	public class ResultsController : Controller
	{
		private readonly ITasksService _tasksClient;

		public ResultsController(ITasksService tasksClient)
		{
			_tasksClient = tasksClient;
		}

		public ActionResult Html(int taskId, int caseId)
		{
			var taskCase = FindTestCaseResult(taskId, caseId);

			if (taskCase == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Could not locate the specified case.");
			}

			var viewModel = new ResultsViewModel
			{
				ActualUrl = taskCase.ActualUrl,
				Content = taskCase.HttpResponse == null ? string.Empty : AddUrlBase(GetBaseUrl(taskCase.ActualUrl), taskCase.HttpResponse.Content)
			};

			return View(viewModel);
		}

		private static string GetBaseUrl(string url)
		{
			Uri result;
			if (Uri.TryCreate(url, UriKind.Absolute, out result))
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}://{1}/", result.Scheme, result.Host);
			}

			return url;
		}

		private string AddUrlBase(string baseUrl, string content)
		{
			// Load content as new Html document
			var html = new HtmlDocument();
			html.LoadHtml(content);

			// Get body node
			HtmlNode body = html.DocumentNode.SelectSingleNode("//body");
			if (body == null)
			{
				body = html.CreateElement("body");
				html.DocumentNode.AppendChild(body);
			}

			var existingBaseElement = body.Element("base");
			if (existingBaseElement != null)
			{
				existingBaseElement.Remove();
			}

			var baseElement = html.CreateElement("base");
			baseElement.SetAttributeValue("url", baseUrl);
			body.PrependChild(baseElement);

			return body.OuterHtml;
		}

		private TestCaseResult FindTestCaseResult(int taskId, int caseId)
		{
			TaskDetails taskDetails = _tasksClient.GetRunningTaskDetails(taskId);

			TestCaseResult taskCase = (from d in taskDetails.Results where d.TestCase.Id == caseId select d).FirstOrDefault();
			return taskCase;
		}

		public ActionResult Raw(int taskId, int caseId)
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