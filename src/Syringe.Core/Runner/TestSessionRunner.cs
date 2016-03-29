using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Logging;
using Syringe.Core.Repositories;
using Syringe.Core.Results;
using Syringe.Core.TestCases;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Core.Runner
{
	public class TestSessionRunner : IObservable<TestCaseResult>
	{
		private readonly IHttpClient _httpClient;
		private bool _isStopPending;
		private List<TestCaseResult> _currentResults;

		private readonly Dictionary<Guid, TestSessionRunnerSubscriber> _subscribers =
			new Dictionary<Guid, TestSessionRunnerSubscriber>();

		public ITestCaseSessionRepository Repository { get; set; }
		public Guid SessionId { get; internal set; }

		public IEnumerable<TestCaseResult> CurrentResults
		{
			get
			{
				lock (_currentResults)
				{
					return _currentResults.AsReadOnly();
				}
			}
		}

		public int CasesRun { get; set; }
		public int TotalCases { get; set; }
		public int RepeatCount { get; set; }

		public TestSessionRunner(IHttpClient httpClient, ITestCaseSessionRepository repository)
		{
			if (httpClient == null)
				throw new ArgumentNullException("httpClient");

			if (repository == null)
				throw new ArgumentNullException("repository");

			_httpClient = httpClient;
			_currentResults = new List<TestCaseResult>();
			Repository = repository;

			SessionId = Guid.NewGuid();
		}

		private void NotifySubscribers(Action<IObserver<TestCaseResult>> observerAction)
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

		private void NotifySubscribersOfAddedResult(TestCaseResult result)
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

		public IDisposable Subscribe(IObserver<TestCaseResult> observer)
		{
			// Notify of the observer of existing results.
			IEnumerable<TestCaseResult> resultsCopy;
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

		public async Task<TestCaseSession> RunAsync(CaseCollection testCollection)
		{
			_isStopPending = false;
			lock (_currentResults)
			{
				_currentResults = new List<TestCaseResult>();
			}

			var session = new TestCaseSession();
			session.TestCaseFilename = testCollection.Filename;
			session.StartTime = DateTime.UtcNow;

			// Add all config variables and ones in this <testcase>
			var variables = new SessionVariables();
			variables.AddOrUpdateVariables(testCollection.Variables);

			var verificationsMatcher = new VerificationsMatcher(variables);

			// Ensure we loop atleast once:
			int repeatTotal = (testCollection.Repeat > 0) ? testCollection.Repeat : 1;
			List<Case> testCases = testCollection.TestCases.ToList();

			TimeSpan minResponseTime = TimeSpan.MaxValue;
			TimeSpan maxResponseTime = TimeSpan.MinValue;
			int totalCasesRun = 0;
			CasesRun = 0;
			TotalCases = testCases.Count;
			RepeatCount = 0;
			bool saveSession = true;

			for (int i = 0; i < repeatTotal; i++)
			{
				foreach (Case testCase in testCases)
				{
					if (_isStopPending)
						break;

					try
					{
						TestCaseResult result = await RunCaseAsync(testCase, variables, verificationsMatcher);
						AddResult(session, result);

						if (result.ResponseTime < minResponseTime)
							minResponseTime = result.ResponseTime;

						if (result.ResponseTime > maxResponseTime)
							maxResponseTime = result.ResponseTime;
					}
					catch (Exception ex)
					{
						Log.Error(ex, "An exception occurred running case {0}", testCase.Id);
						ReportError(ex);
					}
					finally
					{
						totalCasesRun++;
						CasesRun++;
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
			session.TotalCasesRun = totalCasesRun;
			session.MinResponseTime = minResponseTime;
			session.MaxResponseTime = maxResponseTime;

			NotifySubscribersOfCompletion();

			if (saveSession)
			{
				await Repository.AddAsync(session);
			}

			return session;
		}

		private void AddResult(TestCaseSession session, TestCaseResult result)
		{
			session.TestCaseResults.Add(result);
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

		internal async Task<TestCaseResult> RunCaseAsync(Case testCase, SessionVariables variables, VerificationsMatcher verificationMatcher)
		{
			var testResult = new TestCaseResult();
			testResult.SessionId = SessionId;
			testResult.TestCase = testCase;

			try
			{
				string resolvedUrl = variables.ReplacePlainTextVariablesIn(testCase.Url);
				testResult.ActualUrl = resolvedUrl;

				var httpLogWriter = new HttpLogWriter();
				HttpResponse response = await _httpClient.ExecuteRequestAsync(testCase.Method, resolvedUrl, testCase.PostType, testCase.PostBody, testCase.Headers, httpLogWriter);
				testResult.ResponseTime = response.ResponseTime;
				testResult.HttpResponse = response;
				testResult.HttpLog = httpLogWriter.StringBuilder.ToString();
				testResult.HttpContent = response.Content;

				if (response.StatusCode == testCase.VerifyResponseCode)
				{
					testResult.ResponseCodeSuccess = true;
					string content = response.ToString();

					// Put the parseresponse regex values in the current variable set
					var logger = new SimpleLogger();
					logger.WriteLine("");
					logger.WriteLine("Parsing variables");
					logger.WriteLine("--------------------------");
					List<Variable> parsedVariables = ParseResponseMatcher.MatchParseResponses(testCase.ParseResponses, content, logger);
					variables.AddOrUpdateVariables(parsedVariables);
					if (parsedVariables.Count == 0)
					{
						logger.WriteLine("(No variables to parse)");
					}

					// Verify positives
					testResult.VerifyPositiveResults = verificationMatcher.MatchPositive(testCase.VerifyPositives, content);
					logger.WriteLine("");
					logger.WriteLine("Positive verifications");
					logger.WriteLine("--------------------------");
					if (testResult.VerifyPositiveResults.Count > 0)
					{
						foreach (VerificationItem item in testResult.VerifyPositiveResults)
						{
							logger.Write(item.Log);
						}
					}
					else
					{
						logger.WriteLine("(No verify positives found)");
					}

					// Verify Negatives
					testResult.VerifyNegativeResults = verificationMatcher.MatchNegative(testCase.VerifyNegatives, content);
					logger.WriteLine("");
					logger.WriteLine("Negative verifications");
					logger.WriteLine("--------------------------");
					if (testResult.VerifyNegativeResults.Count > 0)
					{
						foreach (VerificationItem item in testResult.VerifyNegativeResults)
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
					testResult.Message = testCase.ErrorMessage;
				}

				// TODO: Inject a delay service for testing purposes (holding up unit tests for orders of seconds is bad).
				if (testCase.Sleep > 0)
					Thread.Sleep(testCase.Sleep * 1000);
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

			public TestSessionRunnerSubscriber(IObserver<TestCaseResult> observer,
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

			public IObserver<TestCaseResult> Observer { get; private set; }

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
