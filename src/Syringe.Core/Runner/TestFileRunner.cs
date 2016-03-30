using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Logging;
using Syringe.Core.Repositories;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Core.Runner
{
	public class TestFileRunner : IObservable<TestResult>
	{
		private readonly IHttpClient _httpClient;
		private bool _isStopPending;
		private List<TestResult> _currentResults;

		private readonly Dictionary<Guid, TestSessionRunnerSubscriber> _subscribers =
			new Dictionary<Guid, TestSessionRunnerSubscriber>();

		public ITestCaseSessionRepository Repository { get; set; }
		public Guid SessionId { get; internal set; }

		public IEnumerable<TestResult> CurrentResults
		{
			get
			{
				lock (_currentResults)
				{
					return _currentResults.AsReadOnly();
				}
			}
		}

		public int TestsRun { get; set; }
		public int TotalTests { get; set; }
		public int RepeatCount { get; set; }

		public TestFileRunner(IHttpClient httpClient, ITestCaseSessionRepository repository)
		{
			if (httpClient == null)
				throw new ArgumentNullException("httpClient");

			if (repository == null)
				throw new ArgumentNullException("repository");

			_httpClient = httpClient;
			_currentResults = new List<TestResult>();
			Repository = repository;

			SessionId = Guid.NewGuid();
		}

		private void NotifySubscribers(Action<IObserver<TestResult>> observerAction)
		{
			IDictionary<Guid, TestSessionRunnerSubscriber> currentSubscribers;
			lock (_subscribers)
			{
				currentSubscribers = _subscribers.ToDictionary(k => k.Key, v => v.Value);
			}

			foreach (var testCaseSessionSubscriber in currentSubscribers.Values)
			{
				observerAction(testCaseSessionSubscriber.Observer);
			}
		}

		private void NotifySubscribersOfAddedResult(TestResult result)
		{
			NotifySubscribers(observer => observer.OnNext(result));
		}

		private void NotifySubscribersOfCompletion()
		{
			NotifySubscribers(observer => observer.OnCompleted());
		}

		public void Stop()
		{
			_isStopPending = true;
		}

		public IDisposable Subscribe(IObserver<TestResult> observer)
		{
			// Notify of the observer of existing results.
			IEnumerable<TestResult> resultsCopy;
			lock (_currentResults)
			{
				resultsCopy = _currentResults.ToArray();
			}

			foreach (var testCaseResult in resultsCopy)
			{
				observer.OnNext(testCaseResult);
			}

			return new TestSessionRunnerSubscriber(observer, _subscribers);
		}

		public async Task<TestFileResult> RunAsync(TestFile testCollection)
		{
			_isStopPending = false;
			lock (_currentResults)
			{
				_currentResults = new List<TestResult>();
			}

			var session = new TestFileResult();
			session.Filename = testCollection.Filename;
			session.StartTime = DateTime.UtcNow;

			// Add all config variables and ones in this <testcase>
			var variables = new CapturedVariableProvider();
			variables.AddOrUpdateVariables(testCollection.Variables);

			var verificationsMatcher = new AssertionsMatcher(variables);

			// Ensure we loop atleast once:
			int repeatTotal = (testCollection.Repeat > 0) ? testCollection.Repeat : 1;
			List<Test> testCases = testCollection.Tests.ToList();

			TimeSpan minResponseTime = TimeSpan.MaxValue;
			TimeSpan maxResponseTime = TimeSpan.MinValue;
			int totalCasesRun = 0;
			TestsRun = 0;
			TotalTests = testCases.Count;
			RepeatCount = 0;
			bool saveSession = true;

			for (int i = 0; i < repeatTotal; i++)
			{
				foreach (Test testCase in testCases)
				{
					if (_isStopPending)
						break;

					try
					{
						TestResult result = await RunCaseAsync(testCase, variables, verificationsMatcher);
						AddResult(session, result);

						if (result.ResponseTime < minResponseTime)
							minResponseTime = result.ResponseTime;

						if (result.ResponseTime > maxResponseTime)
							maxResponseTime = result.ResponseTime;
					}
					catch (Exception ex)
					{
						Log.Error(ex, "An exception occurred running case {0}", testCase.Position);
						ReportError(ex);
					}
					finally
					{
						totalCasesRun++;
						TestsRun++;
						RepeatCount = i;
					}
				}

				if (_isStopPending)
				{
					saveSession = false;
					break;
				}
			}

			session.EndTime = DateTime.UtcNow;
			session.TotalRunTime = session.EndTime - session.StartTime;
			session.TotalTestsRun = totalCasesRun;
			session.MinResponseTime = minResponseTime;
			session.MaxResponseTime = maxResponseTime;

			NotifySubscribersOfCompletion();

			if (saveSession)
			{
				await Repository.AddAsync(session);
			}

			return session;
		}

		private void AddResult(TestFileResult session, TestResult result)
		{
			session.TestResults.Add(result);
			lock (_currentResults)
			{
				_currentResults.Add(result);
			}
			NotifySubscribersOfAddedResult(result);
		}

		public void ReportError(Exception exception)
		{
			NotifySubscribers(observer => observer.OnError(exception));
		}

		internal async Task<TestResult> RunCaseAsync(Test testTest, CapturedVariableProvider variables, AssertionsMatcher assertionMatcher)
		{
			var testResult = new TestResult();
			testResult.SessionId = SessionId;
			testResult.TestTest = testTest;

			try
			{
				string resolvedUrl = variables.ReplacePlainTextVariablesIn(testTest.Url);
				testResult.ActualUrl = resolvedUrl;

				var httpLogWriter = new HttpLogWriter();
				HttpResponse response = await _httpClient.ExecuteRequestAsync(testTest.Method, resolvedUrl, testTest.PostType, testTest.PostBody, testTest.Headers, httpLogWriter);
				testResult.ResponseTime = response.ResponseTime;
				testResult.HttpResponse = response;
				testResult.HttpLog = httpLogWriter.StringBuilder.ToString();
				testResult.HttpContent = response.Content;

				if (response.StatusCode == testTest.VerifyResponseCode)
				{
					testResult.ResponseCodeSuccess = true;
					string content = response.ToString();

					// Put the parseresponse regex values in the current variable set
					var logger = new SimpleLogger();
					logger.WriteLine("");
					logger.WriteLine("Parsing variables");
					logger.WriteLine("--------------------------");
					List<Variable> parsedVariables = CapturedVariableProvider.MatchVariables(testTest.CapturedVariables, content, logger);
					variables.AddOrUpdateVariables(parsedVariables);
					if (parsedVariables.Count == 0)
					{
						logger.WriteLine("(No variables to parse)");
					}

					// Verify positives
					testResult.PositiveAssertionResults = assertionMatcher.MatchPositive(testTest.VerifyPositives, content);
					logger.WriteLine("");
					logger.WriteLine("Positive verifications");
					logger.WriteLine("--------------------------");
					if (testResult.PositiveAssertionResults.Count > 0)
					{
						foreach (Assertion item in testResult.PositiveAssertionResults)
						{
							logger.Write(item.Log);
						}
					}
					else
					{
						logger.WriteLine("(No verify positives found)");
					}

					// Verify Negatives
					testResult.NegativeAssertionResults = assertionMatcher.MatchNegative(testTest.VerifyNegatives, content);
					logger.WriteLine("");
					logger.WriteLine("Negative verifications");
					logger.WriteLine("--------------------------");
					if (testResult.NegativeAssertionResults.Count > 0)
					{
						foreach (Assertion item in testResult.NegativeAssertionResults)
						{
							logger.Write(item.Log);
						}
					}
					else
					{
						logger.WriteLine("(No verify negatives found)");
					}

					// Store the log
					testResult.Log = logger.GetLog();
				}
				else
				{
					testResult.ResponseCodeSuccess = false;
					testResult.Log = "No verifications run - the response code did not match the expected response code.";
				}

				if (testResult.Success == false)
				{
					testResult.Message = testTest.ErrorMessage;
				}
			}
			catch (Exception ex)
			{
				testResult.Log = "An exception occured: " + ex;
				testResult.ResponseCodeSuccess = false;
				testResult.ExceptionMessage = ex.Message;
			}

			return testResult;
		}

		private sealed class TestSessionRunnerSubscriber : IDisposable
		{
			private readonly Guid _key;
			private readonly Dictionary<Guid, TestSessionRunnerSubscriber> _subscriptionList;

			public TestSessionRunnerSubscriber(IObserver<TestResult> observer,
				Dictionary<Guid, TestSessionRunnerSubscriber> subscriptionList)
			{
				Observer = observer;
				_subscriptionList = subscriptionList;
				_key = Guid.NewGuid();

				lock (subscriptionList)
				{
					subscriptionList.Add(_key, this);
				}
			}

			public IObserver<TestResult> Observer { get; private set; }

			public void Dispose()
			{
				lock (_subscriptionList)
				{
					_subscriptionList.Remove(_key);
				}
			}
		}
	}
}
