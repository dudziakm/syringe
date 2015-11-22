using System.Web.Mvc;
using Newtonsoft.Json;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.TestCases;

namespace Syringe.Web.Controllers
{
	public class JsonController : Controller
	{
		private readonly ITasksService _tasksClient;
		private readonly ICaseService _casesClient;
		private readonly IUserContext _userContext;

		public JsonController(ITasksService tasksService, ICaseService casesClient, IUserContext userContext)
		{
			_tasksClient = tasksService;
			_casesClient = casesClient;
			_userContext = userContext;
		}

		public ActionResult Run(string filename)
		{
			var taskRequest = new TaskRequest()
			{
				Filename = filename,
				Username = _userContext.FullName,
				TeamName = _userContext.TeamName,
			};

			int taskId = _tasksClient.Start(taskRequest);

			return Json(new { taskId = taskId });
	    }

		public ActionResult GetProgress(int taskId)
		{
			TaskDetails details = _tasksClient.GetRunningTaskDetails(taskId);

			// Don't use Json() as it fails for large objects.
			return Content(JsonConvert.SerializeObject(details), "application/json");
		}

		public ActionResult GetCases(string filename)
		{
			CaseCollection testCases = _casesClient.GetTestCaseCollection(filename, _userContext.TeamName);
			return Content(JsonConvert.SerializeObject(testCases), "application/json");
		}
	}
}