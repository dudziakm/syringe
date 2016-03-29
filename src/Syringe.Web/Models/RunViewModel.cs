using System;
using System.Collections.Generic;
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

        public RunViewModel(ITasksService tasksService, ICaseService caseService)
        {
            _tasksService = tasksService;
            _caseService = caseService;

	        var mvcConfiguration = new MvcConfiguration();
			SignalRUrl = mvcConfiguration.SignalRUrl;
        }

        public void RunTest(IUserContext userContext, string fileName, int index)
        {
            FileName = fileName;

            var testCase = _caseService.GetTestCase(fileName, userContext.TeamName, index);

            var verifications = new List<VerificationItem>();
            verifications.AddRange(testCase.VerifyNegatives);
            verifications.AddRange(testCase.VerifyPositives);
            _runningTestCases.Add(new RunningTestCaseViewModel(testCase. Position, testCase.ShortDescription, verifications));

            var taskRequest = new TaskRequest
            {
                Filename = fileName,
                Username = userContext.FullName,
                TeamName = userContext.TeamName,
                 Position = index,
            };

            CurrentTaskId = _tasksService.Start(taskRequest);
        }


        public void Run(IUserContext userContext, string fileName)
        {
            FileName = fileName;

            CaseCollection caseCollection = _caseService.GetTestCaseCollection(fileName, userContext.TeamName);

            foreach (var testCase in caseCollection.TestCases)
            {
                var verifications = new List<VerificationItem>();
                verifications.AddRange(testCase.VerifyNegatives);
                verifications.AddRange(testCase.VerifyPositives);
                _runningTestCases.Add(new RunningTestCaseViewModel(testCase. Position, testCase.ShortDescription, verifications));
            }

            var taskRequest = new TaskRequest
            {
                Filename = fileName,
                Username = userContext.FullName,
                TeamName = userContext.TeamName,
            };

            CurrentTaskId = _tasksService.Start(taskRequest);
        }

        public IEnumerable<RunningTestCaseViewModel> TestCases
        {
            get { return _runningTestCases; }
        }

        public int CurrentTaskId { get; private set; }
        public string FileName { get; private set; }
        public string SignalRUrl { get; private set; }
    }
}