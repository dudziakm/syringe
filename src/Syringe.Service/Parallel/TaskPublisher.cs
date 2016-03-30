using System;
using Microsoft.AspNet.SignalR.Hubs;
using Syringe.Core.Tests.Results;
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

		public void Start(int taskId, IObservable<TestResult> resultSource)
		{
			var taskGroup = _taskGroupProvider.GetGroupForTask(taskId);
			resultSource.Subscribe(result => OnTaskCompleted(taskGroup, result));
		}

		private void OnTaskCompleted(string taskGroup, TestResult result)
		{
			var clientGroup = _hubConnectionContext.Group(taskGroup);
		    var verifications = result.NegativeAssertionResults;
            verifications.AddRange(result.PositiveAssertionResults);

			clientGroup.OnTaskCompleted(new CompletedTaskInfo
			{
				ActualUrl = result.ActualUrl,
				HttpResponse = result.HttpResponse,
				Success = result.Success,
				ResultId = result.Id,
				TestId = result.TestTest.Id,
				ExceptionMessage = result.ExceptionMessage,
                Verifications = verifications
            });
		}
	}
}