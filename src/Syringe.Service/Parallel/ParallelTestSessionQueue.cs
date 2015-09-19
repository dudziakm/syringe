using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;
using Syringe.Core.Runner;
using Syringe.Core.Tasks;
using Syringe.Core.TestCases;
using Syringe.Core.TestCases.Configuration;
using Syringe.Core.Xml;
using Syringe.Core.Xml.Reader;

namespace Syringe.Service.Parallel
{
	/// <summary>
	/// A TPL based queue for running XML cases using the default <see cref="TestSessionRunner"/>
	/// </summary>
	internal class ParallelTestSessionQueue
	{
		private readonly ConcurrentBag<Task<SessionRunnerTaskInfo>> _currentTasks;
		private readonly IApplicationConfiguration _appConfig;

		public static ParallelTestSessionQueue Default
		{
			get
			{
				return Nested.Instance;
			}
		}
 
		/// <summary>
		/// This queue is a Singleton, the appdomain only gets one queue.
		/// </summary>
		class Nested
		{
			// "Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit"
			static Nested()
			{
			}
			internal static readonly ParallelTestSessionQueue Instance = new ParallelTestSessionQueue();
		}

		internal ParallelTestSessionQueue()
		{
			_currentTasks = new ConcurrentBag<Task<SessionRunnerTaskInfo>>();
			_appConfig = new ApplicationConfig();
		}

		/// <summary>
		/// Adds a request to run a test case XML file the queue of tasks to run.
		/// </summary>
		public int Add(TaskRequest item)
		{
			Task<SessionRunnerTaskInfo> parentTask = Task.Run(() =>
			{
				var cancelTokenSource = new CancellationTokenSource();
				var cancelToken = cancelTokenSource.Token;

				var taskInfo = new SessionRunnerTaskInfo();
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

				return taskInfo;
			});

			_currentTasks.Add(parentTask);
			return parentTask.Id;
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

					var runner = new TestSessionRunner(config, httpClient, new TextFileResultWriter());
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
			return _currentTasks.Select(task =>
			{
				TestSessionRunner runner = task.Result.Runner;

				return new TaskDetails()
				{
					TaskId = task.Id,
					Username = task.Result.Username,
					TeamName = task.Result.TeamName,
					Status = task.Result.CurrentTask.Status.ToString(),
					CurrentIndex = (runner != null) ? task.Result.Runner.CasesRun : 0,
					TotalCases = (runner != null) ? task.Result.Runner.TotalCases : 0,
				};
			});
		}

		/// <summary>
		/// Shows the full information about a *single* test case run - it doesn't have to be running, it could be complete.
		/// This includes the results of every case in the collection of cases for the XML file run.
		/// </summary>
		public TaskDetails GetRunningTaskDetails(int taskId)
		{
			Task<SessionRunnerTaskInfo> task = _currentTasks.FirstOrDefault(x => x.Id == taskId);
			if (task != null)
			{
				TestSessionRunner runner = task.Result.Runner;
				return new TaskDetails()
				{
					TaskId = task.Id,
					Username = task.Result.Username,
					TeamName = task.Result.TeamName,
					Status = task.Result.CurrentTask.Status.ToString(),
					Results = (runner != null) ? task.Result.Runner.CurrentResults.ToList() : new List<TestCaseResult>(),
					CurrentIndex = (runner != null) ? task.Result.Runner.CasesRun : 0,
					TotalCases = (runner != null) ? task.Result.Runner.TotalCases : 0,
					Errors = task.Result.Errors
				};
			}

			return null;
		}

		/// <summary>
		/// Stops a case XML request task in the queue, returning a message of whether the stop succeeded or not.
		/// </summary>
		public string Stop(int id)
		{
			Task<SessionRunnerTaskInfo> task = _currentTasks.FirstOrDefault(t => t.Id == id);
			if (task != null)
			{
				task.Result.Runner.Stop();
				task.Result.CancelTokenSource.Cancel(false);

				Task<SessionRunnerTaskInfo> result;
				if (_currentTasks.TryTake(out result))
				{
					return string.Format("OK - Task {0} stopped and removed", task.Id);
				}
				else
				{
					return "FAILED - Cancel request made, but removing from the list of tasks failed";
				}
			}
			else
			{
				return string.Format("FAILED - Cannot find task with id {0}", id);
			}
		}

		/// <summary>
		/// Attempts to shut down all running tasks.
		/// </summary>
		public List<string> StopAll()
		{
			List<string> results = new List<string>();
			foreach (Task<SessionRunnerTaskInfo> task in _currentTasks)
			{
				results.Add(Stop(task.Id));
			}

			return results;
		}
	}
}