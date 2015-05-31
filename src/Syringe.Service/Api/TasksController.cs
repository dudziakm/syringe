using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Syringe.Core.Runner;
using Syringe.Service.Models;
using Syringe.Service.Parallel;

namespace Syringe.Service.Api
{
    public class TasksController : ApiController
    {
	    private readonly ParallelTestSessionQueue _sessionQueue;

	    public TasksController()
	    {
		    _sessionQueue = ParallelTestSessionQueue.Default;
	    }

		[Route("api/tasks/Start")]
		[HttpPost]
		public int Start(RunCaseCollectionRequestModel item)
		{
			return _sessionQueue.Add(item);
		}

		[Route("api/tasks/Stop")]
		[HttpGet]
		public string Stop(int id)
		{
			return _sessionQueue.Stop(id);
		}

		[Route("api/tasks/StopAll")]
		[HttpGet]
		public List<string> StopAll()
		{
			return _sessionQueue.StopAll();
		}

		[Route("api/tasks/GetRunningTasks")]
		[HttpGet]
		public IEnumerable<WorkerDetailsModel> GetRunningTasks()
		{
			return _sessionQueue.GetRunningTasks();
		}

		[Route("api/tasks/GetRunningTaskDetails")]
		[HttpGet]
		public WorkerDetailsModel GetRunningTaskDetails(int taskId)
		{
			return _sessionQueue.GetRunningTaskDetails(taskId);
		}
    }
}