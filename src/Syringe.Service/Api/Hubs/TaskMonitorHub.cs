using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Syringe.Core.Results;
using Syringe.Service.Parallel;

namespace Syringe.Service.Api.Hubs
{
    public class TaskMonitorHub : Hub<ITaskMonitorHubClient>
    {
        private readonly ITaskObserver _taskObserver;

        public TaskMonitorHub(ITaskObserver taskObserver)
        {
            _taskObserver = taskObserver;
        }

        public async Task<TaskState> StartMonitoringTask(int taskId)
        {
            await Groups.Add(Context.ConnectionId, GetTaskGroup(taskId));

            TaskMonitoringInfo details = _taskObserver.StartMonitoringTask(taskId);

            if (details == null)
            {
                return new TaskState {TotalCases = 0};
            }

            return new TaskState {TotalCases = details.TotalCases};
        }

        public void SendTaskCompletionNotification(int taskId, TestCaseResult result)
        {
            Clients.Group(GetTaskGroup(taskId))
                .OnTaskCompleted(new CompletedTaskInfo
                {
                    ActualUrl = result.ActualUrl,
                    TaskId = result.Id,
                    Success = result.Success,
                    HttpResponse = result.HttpResponse
                });
        }

        private string GetTaskGroup(int taskId)
        {
            return string.Format(CultureInfo.InvariantCulture, "Task-{0}", taskId);
        }
    }
}
