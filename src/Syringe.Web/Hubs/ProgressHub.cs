using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Syringe.Core.Configuration;

namespace Syringe.Web.Hubs
{
    public class ProgressHub : Hub<IProgressHubClient>
    {
        private readonly IProgressNotificationClient _progressNotificationClient;

        public ProgressHub(IProgressNotificationClient progressNotificationClient)
        {
            _progressNotificationClient = progressNotificationClient;
            _progressNotificationClient.ProgressUpdated += OnProgressUpdated;
        }

        private void OnProgressUpdated(object sender, EventArgs e)
        {
            Clients.All.DoSomething();
        }

        public async Task StartMonitoringProgress(int taskId)
        {
            await _progressNotificationClient.StartMonitoringProgress(taskId);
        }
    }

    public interface IProgressNotificationClient
    {
        event EventHandler ProgressUpdated;
        Task StartMonitoringProgress(int taskId);
    }

    public class SignalRProgressNotifier : IProgressNotificationClient
    {
        private readonly IApplicationConfiguration _applicationConfiguration;

        public event EventHandler ProgressUpdated;

        public SignalRProgressNotifier(IApplicationConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
        }

        public async Task StartMonitoringProgress(int taskId)
        {
            var hubClient = new HubConnection(_applicationConfiguration.ServiceUrl);
            var proxy = hubClient.CreateHubProxy("TaskMonitorHub");
            proxy.On<int>("OnProgressUpdated", id =>
            {
                var handler = ProgressUpdated;
                handler?.Invoke(this, EventArgs.Empty);
            });

            await hubClient.Start();

            await proxy.Invoke("StartMonitoringTask", taskId);
        }
    }

    public interface IProgressHubClient
    {
        void DoSomething();
    }
}