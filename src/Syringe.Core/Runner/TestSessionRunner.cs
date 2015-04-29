using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Logging;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;
using Syringe.Core.Xml;

namespace Syringe.Core.Runner
{
	public class TestSessionRunner
	{
		// TODO: Unit test coverage:
		// - TextWriterResultsWriter [X]
		// - ParsedResponseMatcher
		// - VariableManager
		// - VerificationMatcher
		// - TestSessionRunner
		//   - TestCaseSession is being populated correctly.
		//   - Variables
		//   - Logging: HTTP/Results
		//   - Results
		//   - Runcase
		//   - Repeats
		//   - Start/End time [X]
		
		// TODO: Integration test coverage
		// - RestSharpClient
		//   - Request
		//   - Response
		//   - Log last request + response

		// TODO: Acceptance test coverage
		// - TestSessionRunner
		//   - Roadkill example
		//   - Some kind of REST api one

		// TODO: config.xml takes <testcases>

		private readonly Config _config;
		private readonly IHttpClient _httpClient;
		private readonly IResultWriter _resultWriter;
		private bool _stopPending;

		public TestSessionRunner(Config config, IHttpClient httpClient, IResultWriter resultWriter)
		{
			_config = config;
			_httpClient = httpClient;
			_resultWriter = resultWriter;
		}

		public void Stop()
		{
			_stopPending = true;
		}

		public TestCaseSession Run(ITestCaseReader reader)
		{
			_stopPending = false;
			var session = new TestCaseSession();
			session.StartTime = DateTime.UtcNow;

			CaseCollection testCollection = reader.Read();

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

			for (int i = 0; i < repeatTotal; i++)
			{
				foreach (Case testCase in testCases)
				{
					if (_stopPending)
						break;

					try
					{
						TestCaseResult result = RunCase(testCase, variables, verificationsMatcher);
						session.TestCaseResults.Add(result);

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
					}
				}

				if (_stopPending)
					break;
			}

			session.EndTime = DateTime.UtcNow;
			session.TotalRunTime = session.EndTime - session.StartTime;
			session.TotalCasesRun = totalCasesRun;
			session.MinResponseTime = minResponseTime;
			session.MaxResponseTime = maxResponseTime;

			return session;
		}

		internal TestCaseResult RunCase(Case testCase, SessionVariables variables, VerificationsMatcher verificationMatcher)
		{
			var testResult = new TestCaseResult();
			testResult.TestCase = testCase;

			string resolvedUrl = variables.ReplacePlainTextVariablesIn(testCase.Url);
			testResult.ActualUrl = resolvedUrl;

			HttpResponse response = _httpClient.ExecuteRequest(testCase.Method, resolvedUrl, testCase.PostType, testCase.PostBody, testCase.Headers);
			testResult.ResponseTime = response.ResponseTime;

			if (response.StatusCode == testCase.VerifyResponseCode)
			{
				string content = response.Content;
				testResult.VerifyResponseCodeSuccess = true;

				// Put the parsedresponse regex values in the current variable set
				Dictionary<string, string> parsedVariables = ParsedResponseMatcher.MatchParsedResponses(testCase.ParseResponses, content);
				variables.AddOrUpdateVariables(parsedVariables);

				// Verify positives
				testResult.VerifyPositiveResults = verificationMatcher.MatchPositive(testCase.VerifyPositives, content);

				// Verify Negatives
				testResult.VerifyNegativeResults = verificationMatcher.MatchNegative(testCase.VerifyNegatives, content);
			}
			else
			{
				testResult.VerifyResponseCodeSuccess = false;
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

			_resultWriter.Write(testResult);

			if (testCase.Sleep > 0)
				Thread.Sleep(testCase.Sleep);

			return testResult;
		}

		internal bool ShouldLogRequest(TestCaseResult testResult, Case testCase)
		{
			return (testResult.VerifyResponseCodeSuccess == false && _config.GlobalHttpLog == LogType.OnFail)
			       || _config.GlobalHttpLog == LogType.All 
				   || testCase.LogRequest;
		}

		internal bool ShouldLogResponse(TestCaseResult testResult, Case testCase)
		{
			return (testResult.VerifyResponseCodeSuccess == false && _config.GlobalHttpLog == LogType.OnFail)
				   || _config.GlobalHttpLog == LogType.All
				   || testCase.LogResponse;
		}
	}
}
