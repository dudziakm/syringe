using System.Collections.Generic;
using System.Web.Http;
using Syringe.Service.Models;
using Syringe.Service.Parallel;

namespace Syringe.Service.Controllers
{
    public class TasksController : ApiController
    {
		[Route("api/Start")]
		[HttpPost]
		public void Start(RunCaseCollectionRequestModel item)
		{
			ParallelCaseQueue.Default.Add(item);
		}

		[Route("api/tasks/Stop")]
		[HttpGet]
		public string Stop(int id)
		{
			return ParallelCaseQueue.Default.Stop(id);
		}

		[Route("api/tasks/StopAll")]
		[HttpGet]
		public List<string> StopAll()
		{
			return ParallelCaseQueue.Default.StopAll();
		}

		[Route("api/tasks/GetRunningTasks")]
		[HttpGet]
		public IEnumerable<WorkerDetailsModel> GetRunningTasks()
		{
			return ParallelCaseQueue.Default.GetRunningTasks();
		}

		[Route("api/tasks/GetRunningTaskDetails")]
		[HttpGet]
		public WorkerDetailsModel GetRunningTaskDetails(int taskId)
		{
			return ParallelCaseQueue.Default.GetRunningTaskDetails(taskId);
		}
    }
}