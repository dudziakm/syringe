using Microsoft.AspNet.SignalR;

namespace Syringe.Service.Api.Hubs
{
    public class TaskMonitorHub : Hub<ITaskMonitorHubClient>
    {
        public void StartMonitoringTask(int taskId)
        {
            Clients.All.OnProgressUpdated(taskId);
        }
    }

    public interface ITaskMonitorHubClient
    {
        void OnProgressUpdated(int taskId);
    }
}
