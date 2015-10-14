using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Repositories;
using Syringe.Core.Results;
using Syringe.Core.Runner;
using Syringe.Core.Tasks;
using Syringe.Core.TestCases;
using Syringe.Core.TestCases.Configuration;
using Syringe.Core.Xml.Reader;

namespace Syringe.Service.Parallel
{
	/// <summary>
	/// A TPL based queue for running XML cases using the default <see cref="TestSessionRunner"/>
	/// </summary>
	internal class ParallelTestSessionQueue : ITestSessionQueue, ITaskObserver
	{
		private int _lastTaskId;
		private readonly ConcurrentDictionary<int, SessionRunnerTaskInfo> _currentTasks;
		private readonly IApplicationConfiguration _appConfig;
		private readonly ITestCaseSessionRepository _repository;

		public ParallelTestSessionQueue(ITestCaseSessionRepository repository)
		{
			_currentTasks = new ConcurrentDictionary<int, SessionRunnerTaskInfo>();
			_appConfig = new ApplicationConfig();

			_repository = repository;
		}

		/// <summary>
		/// Adds a request to run a test case XML file the queue of tasks to run.
		/// </summary>
		public int Add(TaskRequest item)
		{
			int taskId = Interlocked.Increment(ref _lastTaskId);

			var cancelTokenSource = new CancellationTokenSource();
			var cancelToken = cancelTokenSource.Token;

			var taskInfo = new SessionRunnerTaskInfo(taskId);
			taskInfo.Request = item;
			taskInfo.StartTime = DateTime.UtcNow;
			taskInfo.Username = item.Username;
			taskInfo.TeamName = item.TeamName;

			Task childTask = Task.Run(() =>
			{
				StartSession(taskInfo);
			}, cancelToken);

			taskInfo.CancelTokenSource = cancelTokenSource;
			taskInfo.CurrentTask = childTask;

			_currentTasks.TryAdd(taskId, taskInfo);
			return taskId;
		}

		/// <summary>
		/// Starts the test case XML file run.
		/// </summary>
		internal void StartSession(SessionRunnerTaskInfo item)
		{
			try
			{
				// TODO: this run could be for a user run only, not the entire team (read the XML from their folder?)
				string username = item.Username;
				string teamName = item.TeamName;

				// Read in the XML file from the team folder, e.g. "c:\testcases\myteam\test1.xml"
				string xmlFilename = item.Request.Filename;
				string fullPath = Path.Combine(_appConfig.TestCasesBaseDirectory, teamName, xmlFilename);
				string xml = File.ReadAllText(fullPath);

				using (var stringReader = new StringReader(xml))
				{
					var testCaseReader = new TestCaseReader();
					CaseCollection caseCollection = testCaseReader.Read(stringReader);
					caseCollection.Filename = xmlFilename;
					var config = new Config();
					var logStringBuilder = new StringBuilder();
					var httpLogWriter = new HttpLogWriter(new StringWriter(logStringBuilder));
					var httpClient = new HttpClient(httpLogWriter, new RestClient());

					var runner = new TestSessionRunner(config, httpClient, _repository);
					item.Runner = runner;
					runner.Run(caseCollection);
				}
			}
			catch (Exception e)
			{
				item.Errors = e.ToString();
			}
		}

		/// <summary>
		/// Shows minimal information about all test case XML file requests in the queue, and their status,
		/// and who started the run.
		/// </summary>
		public IEnumerable<TaskDetails> GetRunningTasks()
		{
			return _currentTasks.Values.Select(task =>
			{
				TestSessionRunner runner = task.Runner;

				return new TaskDetails()
				{
					TaskId = task.Id,
					Username = task.Username,
					TeamName = task.TeamName,
					Status = task.CurrentTask.Status.ToString(),
					CurrentIndex = (runner != null) ? task.Runner.CasesRun : 0,
					TotalCases = (runner != null) ? task.Runner.TotalCases : 0,
				};
			});
		}

		/// <summary>
		/// Shows the full information about a *single* test case run - it doesn't have to be running, it could be complete.
		/// This includes the results of every case in the collection of cases for the XML file run.
		/// </summary>
		public TaskDetails GetRunningTaskDetails(int taskId)
		{
			SessionRunnerTaskInfo task;
			_currentTasks.TryGetValue(taskId, out task);
			if (task == null)
			{
				return null;
			}

			TestSessionRunner runner = task.Runner;
			return new TaskDetails()
			{
				TaskId = task.Id,
				Username = task.Username,
				TeamName = task.TeamName,
				Status = task.CurrentTask.Status.ToString(),
				Results = (runner != null) ? runner.CurrentResults.ToList() : new List<TestCaseResult>(),
				CurrentIndex = (runner != null) ? runner.CasesRun : 0,
				TotalCases = (runner != null) ? runner.TotalCases : 0,
				Errors = task.Errors
			};
		}

		/// <summary>
		/// Stops a case XML request task in the queue, returning a message of whether the stop succeeded or not.
		/// </summary>
		public string Stop(int taskId)
		{
			SessionRunnerTaskInfo task;
			_currentTasks.TryRemove(taskId, out task);
			if (task == null)
			{
				return "FAILED - Cancel request made, but removing from the list of tasks failed";
			}


			task.Runner.Stop();
			task.CancelTokenSource.Cancel(false);

			return string.Format("OK - Task {0} stopped and removed", task.Id);
		}

		/// <summary>
		/// Attempts to shut down all running tasks.
		/// </summary>
		public List<string> StopAll()
		{
			List<string> results = new List<string>();
			foreach (SessionRunnerTaskInfo task in _currentTasks.Values)
			{
				results.Add(Stop(task.Id));
			}

			return results;
		}

		public TaskMonitoringInfo StartMonitoringTask(int taskId)
		{
			SessionRunnerTaskInfo task;
			_currentTasks.TryGetValue(taskId, out task);
			if (task == null)
			{
				return null;
			}

			TestSessionRunner runner = task.Runner;

			return new TaskMonitoringInfo(runner.TotalCases);
		}
	}
}