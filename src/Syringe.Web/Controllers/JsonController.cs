using System;
using System.Web.Mvc;
using RestSharp;
using Syringe.Core;
using Syringe.Core.ApiClient;
using Syringe.Core.Domain.Entities;
using Syringe.Core.Runner;
using Syringe.Core.Security;

namespace Syringe.Web.Controllers
{
	public class JsonController : Controller
	{
		private readonly TasksClient _tasksClient;
		private readonly CasesClient _casesClient;
		private readonly IUserContext _userContext;

		public JsonController()
		{
			_tasksClient = new TasksClient();
			_casesClient = new CasesClient();
			_userContext = new UserContext();
		}

		public ActionResult Run(string filename)
		{
			var taskRequest = new TaskRequest()
			{
				Filename = filename,
				Username = _userContext.Username,
				TeamName = _userContext.TeamName,
			};

			int taskId = _tasksClient.Start(taskRequest);

			return Json(new { taskId = taskId });
	    }

		public ActionResult GetProgress(int taskId)
		{
			TaskDetails details = _tasksClient.GetRunningTaskDetails(taskId);
			return Json(details, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetCases(string filename)
		{
			CaseCollection testCases = _casesClient.GetTestCaseCollection(filename, _userContext.TeamName);
			return Json(testCases, JsonRequestBehavior.AllowGet);
		}
	}
}