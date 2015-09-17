using System.Collections.Generic;
using Syringe.Core.Tasks;

namespace Syringe.Core.Services
{
	public interface ITasksService
	{
		int Start(TaskRequest item);
		string Stop(int id);
		List<string> StopAll();
		IEnumerable<TaskDetails> GetRunningTasks();
		TaskDetails GetRunningTaskDetails(int taskId);
	}
}