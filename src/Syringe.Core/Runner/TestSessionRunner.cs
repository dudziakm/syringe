using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Results;
using Syringe.Core.Xml;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Core.Runner
{
	public class TestSessionRunner
	{
		// TODO: variables on URL
		// TODO: ResultWriter for results output (StdOut,Xml etc.)
		// TODO: config.xml takes <testcases>

		private readonly Config _config;
		private readonly IHttpClient _httpClient;
		private readonly IHttpLogWriter _logWriter;

		public TestSessionRunner(Config config, IHttpClient httpClient, IHttpLogWriter logWriter)
		{
			_config = config;
			_httpClient = httpClient;
			_logWriter = logWriter;
		}

		public TestCaseSession Run(ITestCaseReader reader, TextReader textReader)
		{
			var runSummary = new TestCaseSession();
			runSummary.StartTime = DateTime.UtcNow;

			CaseCollection testCollection = reader.Read(textReader);

			var variableManager = new VariableManager();
			variableManager.AddGlobalVariables(_config);

			var verificationMatcher = new VerificationMatcher(variableManager);

			foreach (Case testCase in testCollection.TestCases)
			{
				Console.WriteLine("==== Testcase {0}", testCase.Id);

				var testResult = new TestCaseResult();
				testResult.TestCase = testCase;

				HttpResponse response = _httpClient.ExecuteRequest(testCase.Method, testCase.Url, testCase.PostType, testCase.PostBody, testCase.Headers);
				testResult.ResponseTime = response.ResponseTime;

				if (response.StatusCode == testCase.VerifyResponseCode)
				{
					string content = response.Content;
					testResult.Success = true;

					// Put the parsedresponse regex values in the current variable set
					Dictionary<string, string> variables = ParsedResponseMatcher.MatchParsedResponses(testCase.ParseResponses, content);
					variableManager.AddOrUpdateVariables(variables);

					// Verify positives
					testResult.VerifyPositiveResults = verificationMatcher.MatchPositiveVerifications(testCase.VerifyPositives, content);

					// Verify Negatives
					testResult.VerifyNegativeResults = verificationMatcher.MatchPositiveVerifications(testCase.VerifyNegatives, content);
				}
				else
				{
					testResult.Message = testCase.ErrorMessage;
					testResult.Success = false;
				}

				if (_config.GlobalHttpLog != LogType.None)
				{
					LogToWriter(testResult, testCase, response);
				}

				if (testCase.Sleep > 0)
					Thread.Sleep(testCase.Sleep);
			}

			return runSummary;
		}

		private void LogToWriter(TestCaseResult testResult, Case testCase, HttpResponse response)
		{
			// Log request
			if (testResult.Success == false && _config.GlobalHttpLog == LogType.OnFail)
			{
				_logWriter.AppendRequest(testCase.Method, testCase.Url, testCase.Headers);
			}
			else if (_config.GlobalHttpLog == LogType.All || testCase.LogRequest)
			{
				_logWriter.AppendRequest(testCase.Method, testCase.Url, testCase.Headers);
			}

			// Log response
			if (testResult.Success == false && _config.GlobalHttpLog == LogType.OnFail)
			{
				_logWriter.AppendResponse(response.StatusCode, response.Headers, response.Content);
			}
			else if (_config.GlobalHttpLog == LogType.All || testCase.LogResponse)
			{
				_logWriter.AppendResponse(response.StatusCode, response.Headers, response.Content);
			}

			_logWriter.AppendSeperator();
		}
	}
}
