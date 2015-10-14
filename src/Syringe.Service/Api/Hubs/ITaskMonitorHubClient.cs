using System;
using Syringe.Core.Http;

namespace Syringe.Service.Api.Hubs
{
    public interface ITaskMonitorHubClient
    {
        void OnTaskCompleted(CompletedTaskInfo taskInfo);
    }

    public class CompletedTaskInfo
    {
        public string ActualUrl { get; set; }
        public Guid TaskId { get; set; }
        public bool Success { get; set; }
        public HttpResponse HttpResponse { get; set; }
    }
}