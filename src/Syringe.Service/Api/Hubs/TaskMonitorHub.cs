using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Syringe.Service.Api.Hubs
{
    public class TaskMonitorHub : Hub<ITaskMonitorHubClient>
    {
        public async Task StartMonitoringTask(int taskId, IProgress<TaskState> progress)
        {
            progress.Report(new TaskState { TotalTasks = 5, CompletedTasks = 0 });
            await Task.Delay(100);

            progress.Report(new TaskState { TotalTasks = 5, CompletedTasks = 2 });
            await Task.Delay(100);

            progress.Report(new TaskState { TotalTasks = 5, CompletedTasks = 4 });
            await Task.Delay(100);

            progress.Report(new TaskState { TotalTasks = 5, CompletedTasks = 5 });
            await Task.Delay(100);
        }
    }

    public class TaskState
    {
        public int CompletedTasks { get; set; }
        public int TotalTasks { get; set; }
    }

    public interface ITaskMonitorHubClient
    {
        void OnProgressUpdated(int taskId);
    }
}
