using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;
using Syringe.Core.Xml;

namespace Syringe.Core.Runner
{
	public class TestSessionRunner
	{
		// TODO: Unit test coverage:
		// - TextWriterResultsWriter
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
		//   - Start/End time
		
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
			var runSummary = new TestCaseSession();
			runSummary.StartTime = DateTime.UtcNow;

			CaseCollection testCollection = reader.Read();

			var variableManager = new VariableManager();
			variableManager.AddGlobalVariables(_config);
			variableManager.AddOrUpdateVariables(testCollection.Variables);

			var verificationMatcher = new VerificationMatcher(variableManager);

			// Ensure we loop atleast once:
			int repeatTotal = (testCollection.Repeat > 0) ? testCollection.Repeat : 1;
			List<Case> testCases = testCollection.TestCases.ToList();
			for (int i = 0; i < repeatTotal; i++)
			{
				foreach (Case testCase in testCases)
				{
					TestCaseResult result = RunCase(testCase, variableManager, verificationMatcher);
					runSummary.TestCaseResults.Add(result);

					if (_stopPending)
						break;
				}

				if (_stopPending)
					break;
			}

			runSummary.EndTime = DateTime.UtcNow;

			return runSummary;
		}

		private TestCaseResult RunCase(Case testCase, VariableManager variableManager, VerificationMatcher verificationMatcher)
		{
			var testResult = new TestCaseResult();
			testResult.TestCase = testCase;

			string resolvedUrl = variableManager.ReplacePlainTextVariablesIn(testCase.Url);
			testResult.ActualUrl = resolvedUrl;

			HttpResponse response = _httpClient.ExecuteRequest(testCase.Method, resolvedUrl, testCase.PostType, testCase.PostBody,
				testCase.Headers);
			testResult.ResponseTime = response.ResponseTime;

			if (response.StatusCode == testCase.VerifyResponseCode)
			{
				string content = response.Content;
				testResult.VerifyResponseCodeSuccess = true;

				// Put the parsedresponse regex values in the current variable set
				Dictionary<string, string> variables = ParsedResponseMatcher.MatchParsedResponses(testCase.ParseResponses, content);
				variableManager.AddOrUpdateVariables(variables);

				// Verify positives
				testResult.VerifyPositiveResults = verificationMatcher.MatchPositiveVerifications(testCase.VerifyPositives, content);

				// Verify Negatives
				testResult.VerifyNegativeResults = verificationMatcher.MatchNegativeVerifications(testCase.VerifyNegatives, content);
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

		private bool ShouldLogRequest(TestCaseResult testResult, Case testCase)
		{
			return (testResult.VerifyResponseCodeSuccess == false && _config.GlobalHttpLog == LogType.OnFail)
			       || _config.GlobalHttpLog == LogType.All 
				   || testCase.LogRequest;
		}

		private bool ShouldLogResponse(TestCaseResult testResult, Case testCase)
		{
			return (testResult.VerifyResponseCodeSuccess == false && _config.GlobalHttpLog == LogType.OnFail)
				   || _config.GlobalHttpLog == LogType.All
				   || testCase.LogResponse;
		}
	}
}
