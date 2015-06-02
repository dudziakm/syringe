using System.Collections.Generic;
using Syringe.Core.Domain.Entities;
using Syringe.Core.Runner;

namespace Syringe.Core.Domain.Service
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