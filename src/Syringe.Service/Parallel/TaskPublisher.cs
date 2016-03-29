using System;
using Microsoft.AspNet.SignalR.Hubs;
using Syringe.Core.Results;
using Syringe.Service.Api.Hubs;

namespace Syringe.Service.Parallel
{
	public class TaskPublisher : ITaskPublisher
	{
		private readonly ITaskGroupProvider _taskGroupProvider;
		private readonly IHubConnectionContext<ITaskMonitorHubClient> _hubConnectionContext;

		public TaskPublisher(ITaskGroupProvider taskGroupProvider, IHubConnectionContext<ITaskMonitorHubClient> hubConnectionContext)
		{
			_taskGroupProvider = taskGroupProvider;
			_hubConnectionContext = hubConnectionContext;
		}

		public void Start(int taskId, IObservable<TestCaseResult> resultSource)
		{
			var taskGroup = _taskGroupProvider.GetGroupForTask(taskId);
			resultSource.Subscribe(result => OnTaskCompleted(taskGroup, result));
		}

		private void OnTaskCompleted(string taskGroup, TestCaseResult result)
		{
			var clientGroup = _hubConnectionContext.Group(taskGroup);
		    var verifications = result.VerifyNegativeResults;
            verifications.AddRange(result.VerifyPositiveResults);

			clientGroup.OnTaskCompleted(new CompletedTaskInfo
			{
				ActualUrl = result.ActualUrl,
				HttpResponse = result.HttpResponse,
				Success = result.Success,
				ResultId = result.Id,
				 Position = result.TestCase.Position,
				ExceptionMessage = result.ExceptionMessage,
                Verifications = verifications
            });
		}
	}
}