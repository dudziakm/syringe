using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Raven.Client.Document;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Logging;
using Syringe.Core.Repositories.RavenDB;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;
using Syringe.Core.TestCases;
using Syringe.Core.TestCases.Configuration;
using Syringe.Core.Xml;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Core.Runner
{
	public class TestSessionRunner
	{
		// TODO: features
		//   - config.xml takes <testcases>

		// TODO: Unit test coverage:
		// - HttpClient
		//   - Request
		//   - Response
		//   - Log last request + response

		// TODO: Acceptance test coverage
		// - TestSessionRunner
		//   - Roadkill example
		//   - Some kind of REST api one

		private readonly Config _config;
		private readonly IHttpClient _httpClient;
		private readonly IResultWriter _resultWriter;
		private bool _isStopPending;
		private List<TestCaseResult> _currentResults;

		public Guid SessionId { get; internal set; }

		public IEnumerable<TestCaseResult> CurrentResults
		{
			get { return _currentResults; }
		}

		public int CasesRun { get; set; }
		public int TotalCases { get; set; }
		public int RepeatCount { get; set; }

		public TestSessionRunner(Config config, IHttpClient httpClient, IResultWriter resultWriter)
		{
			if (config == null)
				throw new ArgumentNullException("config");

			if (httpClient == null)
				throw new ArgumentNullException("httpClient");

			if (resultWriter == null)
				throw new ArgumentNullException("resultWriter");

			_config = config;
			_httpClient = httpClient;
			_resultWriter = resultWriter;
			_currentResults = new List<TestCaseResult>();
			SessionId = Guid.NewGuid();
		}

		/// <summary>
		/// Creates a new <see cref="TestSessionRunner"/> using the defaults.
		/// </summary>
		/// <returns></returns>
		public static TestSessionRunner CreateNew()
		{
			var config = new Config();
			var logStringBuilder = new StringBuilder();
			var httpLogWriter = new HttpLogWriter(new StringWriter(logStringBuilder));
			var httpClient = new HttpClient(httpLogWriter, new RestClient());

			return new TestSessionRunner(config, httpClient, new TextFileResultWriter());
		}

		public void Stop()
		{
			_isStopPending = true;
		}

		public TestCaseSession Run(CaseCollection testCollection)
		{
			_isStopPending = false;
			_currentResults = new List<TestCaseResult>();

			var session = new TestCaseSession();
			session.TestCaseFilename = testCollection.Filename;
			session.StartTime = DateTime.UtcNow;

			// Add all config variables and ones in this <testcase>
			var variables = new SessionVariables();
			variables.AddGlobalVariables(_config);
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
						TestCaseResult result = RunCase(testCase, variables, verificationsMatcher);
						session.TestCaseResults.Add(result);
						_currentResults.Add(result);

						if (result.ResponseTime < minResponseTime)
							minResponseTime = result.ResponseTime;

						if (result.ResponseTime > maxResponseTime)
							maxResponseTime = result.ResponseTime;
					}
					catch (Exception ex)
					{
						Log.Error(ex, "An exception occurred running case {0}", testCase.Id);
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

			if (saveSession)
			{
				var ravenDbConfig = new RavenDBConfiguration();
				using (var documentStore = new DocumentStore() { Url = ravenDbConfig.Url, DefaultDatabase = ravenDbConfig.DefaultDatabase })
				{
					using (var repository = new RavenDbTestCaseSessionRepository(documentStore))
					{
						repository.Save(session);
					}
				}
			}

			return session;
		}

		internal TestCaseResult RunCase(Case testCase, SessionVariables variables, VerificationsMatcher verificationMatcher)
		{
			var testResult = new TestCaseResult();
			testResult.SessionId = SessionId;
			testResult.TestCase = testCase;

			try
			{
				string resolvedUrl = variables.ReplacePlainTextVariablesIn(testCase.Url);
				testResult.ActualUrl = resolvedUrl;

				HttpResponse response = _httpClient.ExecuteRequest(testCase.Method, resolvedUrl, testCase.PostType, testCase.PostBody, testCase.Headers);
				testResult.ResponseTime = response.ResponseTime;
				testResult.HttpResponse = response;

				if (response.StatusCode == testCase.VerifyResponseCode)
				{
					testResult.ResponseCodeSuccess = true;
					string content = response.ToString();

					// Put the parseresponse regex values in the current variable set
					Dictionary<string, string> parsedVariables = ParseResponseMatcher.MatchParseResponses(testCase.ParseResponses, content);
					variables.AddOrUpdateVariables(parsedVariables);

					// Verify positives
					testResult.VerifyPositiveResults = verificationMatcher.MatchPositive(testCase.VerifyPositives, content);

					// Verify Negatives
					testResult.VerifyNegativeResults = verificationMatcher.MatchNegative(testCase.VerifyNegatives, content);
				}
				else
				{
					testResult.ResponseCodeSuccess = false;
				}

				if (testResult.Success == false)
				{
					testResult.Message = testCase.ErrorMessage;
				}

				if (ShouldLogRequest(testResult, testCase))
				{
					_httpClient.LogLastRequest();
				}

				if (ShouldLogResponse(testResult, testCase))
				{
					_httpClient.LogLastResponse();
				}

				if (testCase.Sleep > 0)
					Thread.Sleep(testCase.Sleep * 1000);
			}
			catch (Exception ex)
			{
				testResult.ResponseCodeSuccess = false;
				testResult.ExceptionMessage = ex.Message;
			}

			return testResult;
		}

		internal bool ShouldLogRequest(TestCaseResult testResult, Case testCase)
		{
			return (testResult.ResponseCodeSuccess == false && _config.GlobalHttpLog == LogType.OnFail)
				   || _config.GlobalHttpLog == LogType.All
				   || testCase.LogRequest;
		}

		internal bool ShouldLogResponse(TestCaseResult testResult, Case testCase)
		{
			return (testResult.ResponseCodeSuccess == false && _config.GlobalHttpLog == LogType.OnFail)
				   || _config.GlobalHttpLog == LogType.All
				   || testCase.LogResponse;
		}
	}
}
