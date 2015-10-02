using System.Collections.Generic;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.TestCases;

namespace Syringe.Web.Models
{
	public class RunViewModel : IRunViewModel
	{
		private readonly ITasksService _tasksService;
		private readonly ICaseService _caseService;
	    private readonly List<RunningTestCaseViewModel> _runningTestCases = new List<RunningTestCaseViewModel>();

		public RunViewModel(ITasksService tasksService, ICaseService caseService, IApplicationConfiguration applicationConfiguration)
		{
			_tasksService = tasksService;
			_caseService = caseService;
		    SignalRUrl = applicationConfiguration.SignalRUrl;
		}

		public void Run(IUserContext userContext, string fileName)
		{
			FileName = fileName;

			CaseCollection caseCollection = _caseService.GetTestCaseCollection(fileName, userContext.TeamName);

			foreach (var testCase in caseCollection.TestCases)
			{
				_runningTestCases.Add(new RunningTestCaseViewModel(testCase.Id, testCase.ShortDescription));
			}

			var taskRequest = new TaskRequest
			{
				Filename = fileName,
				Username = userContext.Username,
				TeamName = userContext.TeamName,
			};

			CurrentTaskId = _tasksService.Start(taskRequest);
		}

		public IEnumerable<RunningTestCaseViewModel> TestCases => _runningTestCases;

		public int CurrentTaskId { get; private set; }
		public string FileName { get; private set; }
	    public string SignalRUrl { get; private set; }
	}
}