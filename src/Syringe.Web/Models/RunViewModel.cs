using System.Collections.Generic;
using Syringe.Core.Configuration;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.Tests;

namespace Syringe.Web.Models
{
    public class RunViewModel : IRunViewModel
    {
        private readonly ITasksService _tasksService;
        private readonly ICaseService _caseService;
        private readonly List<RunningTestFileViewModel> _runningTestCases = new List<RunningTestFileViewModel>();

        public RunViewModel(ITasksService tasksService, ICaseService caseService)
        {
            _tasksService = tasksService;
            _caseService = caseService;

	        var mvcConfiguration = new MvcConfiguration();
			SignalRUrl = mvcConfiguration.SignalRUrl;
        }

        public void Run(IUserContext userContext, string fileName)
        {
            FileName = fileName;

            TestFile testFile = _caseService.GetTestCaseCollection(fileName, userContext.TeamName);

            foreach (var testCase in testFile.Tests)
            {
                var verifications = new List<Assertion>();
                verifications.AddRange(testCase.VerifyNegatives);
                verifications.AddRange(testCase.VerifyPositives);
                _runningTestCases.Add(new RunningTestFileViewModel(testCase.Id, testCase.ShortDescription, verifications));
            }

            var taskRequest = new TaskRequest
            {
                Filename = fileName,
                Username = userContext.FullName,
                TeamName = userContext.TeamName,
            };

            CurrentTaskId = _tasksService.Start(taskRequest);
        }

        public IEnumerable<RunningTestFileViewModel> TestCases
        {
            get { return _runningTestCases; }
        }

        public int CurrentTaskId { get; private set; }
        public string FileName { get; private set; }
        public string SignalRUrl { get; private set; }
    }
}