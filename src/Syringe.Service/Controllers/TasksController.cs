using System.Collections.Generic;
using System.Web.Http;
using Syringe.Service.Models;
using Syringe.Service.Parallel;

namespace Syringe.Service.Controllers
{
    public class TasksController : ApiController
    {
	    private readonly ParallelTestSessionQueue _caseQueue;

	    public TasksController()
	    {
		    _caseQueue = ParallelTestSessionQueue.Default;
	    }

		[Route("api/Start")]
		[HttpPost]
		public void Start(RunCaseCollectionRequestModel item)
		{
			_caseQueue.Add(item);
		}

		[Route("api/tasks/Stop")]
		[HttpGet]
		public string Stop(int id)
		{
			return _caseQueue.Stop(id);
		}

		[Route("api/tasks/StopAll")]
		[HttpGet]
		public List<string> StopAll()
		{
			return _caseQueue.StopAll();
		}

		[Route("api/tasks/GetRunningTasks")]
		[HttpGet]
		public IEnumerable<WorkerDetailsModel> GetRunningTasks()
		{
			return _caseQueue.GetRunningTasks();
		}

		[Route("api/tasks/GetRunningTaskDetails")]
		[HttpGet]
		public WorkerDetailsModel GetRunningTaskDetails(int taskId)
		{
			return _caseQueue.GetRunningTaskDetails(taskId);
		}
    }
}