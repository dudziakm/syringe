using System.Collections.Generic;
using System.Web.Http;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Service.Parallel;

namespace Syringe.Service.Api
{
	// TODO: Tests
	public class TasksController : ApiController, ITasksService
	{
	    private readonly ITestSessionQueue _sessionQueue;

	    public TasksController()
	    {
		    _sessionQueue = ParallelTestSessionQueue.Default;
	    }

		[Route("api/tasks/Start")]
		[HttpPost]
		public int Start(TaskRequest item)
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
		public IEnumerable<TaskDetails> GetRunningTasks()
		{
			return _sessionQueue.GetRunningTasks();
		}

		[Route("api/tasks/GetRunningTaskDetails")]
		[HttpGet]
		public TaskDetails GetRunningTaskDetails(int taskId)
		{
			return _sessionQueue.GetRunningTaskDetails(taskId);
		}
    }
}