using System;
using System.Web.Mvc;
using RestSharp;
using Syringe.Core;
using Syringe.Web.Client;

namespace Syringe.Web.Controllers
{
	public class JsonController : Controller
	{
		private readonly TasksClient _tasksClient;
		private readonly CasesClient _casesClient;

		public JsonController()
		{
			_tasksClient = new TasksClient();
			_casesClient = new CasesClient();
		}

		public ActionResult Run(string filename)
		{
			int taskId = _tasksClient.Start(filename, "username TODO");
			return Json(new { taskId = taskId });
	    }

		public ActionResult GetProgress(int taskId)
		{
			WorkerDetails details = _tasksClient.GetProgress(taskId);
			return Json(details, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetCases(string filename)
		{
			CaseCollection testCases = _casesClient.GetCaseCollection(filename);
			return Json(testCases, JsonRequestBehavior.AllowGet);
		}
	}
}