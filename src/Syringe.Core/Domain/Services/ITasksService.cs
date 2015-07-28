using System.Collections.Generic;
using Syringe.Core.Domain.Entities;

namespace Syringe.Core.Domain.Services
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