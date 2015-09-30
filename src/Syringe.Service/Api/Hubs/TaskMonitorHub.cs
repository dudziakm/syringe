using Microsoft.AspNet.SignalR;
using Syringe.Core.Configuration;

namespace Syringe.Service.Api.Hubs
{
    public class TaskMonitorHub : Hub<ITaskMonitorHubClient>
    {
        public TaskMonitorHub(IApplicationConfiguration dummy)
        {
            
        }

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
