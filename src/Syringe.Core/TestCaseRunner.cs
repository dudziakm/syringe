using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using RestSharp;
using RestSharp.Contrib;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Results;
using Syringe.Core.Xml;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Core
{
	public class TestCaseRunner
	{
		// TODO: variables on URL
		// TODO: ResultWriter for results output (StdOut,Xml etc.)
		// TODO: config.xml takes <testcases>

		private readonly Config _config;
		private readonly IHttpClient _httpClient;
		private readonly IHttpLogWriter _logWriter;

		public TestCaseRunner(Config config, IHttpClient httpClient, IHttpLogWriter logWriter)
		{
			_config = config;
			_httpClient = httpClient;
			_logWriter = logWriter;
		}

		public TestCaseRunSummary Run(string testCaseFilename)
		{
			var runSummary = new TestCaseRunSummary();
			runSummary.StartTime = DateTime.UtcNow;

			using (var stringReader = new StringReader(File.ReadAllText(testCaseFilename)))
			{
				var testCaseReader = new LegacyTestCaseReader();
				TestCaseCollection testCollection = testCaseReader.Read(stringReader);

				foreach (TestCase testCase in testCollection.TestCases)
				{
					var testResult = new TestCaseResult();
					testResult.TestCase = testCase;

					HttpResponse response = _httpClient.ExecuteRequest(testCase.Method, testCase.Url, testCase.PostType, testCase.PostBody, testCase.AddHeader);
					testResult.ResponseTime = response.ResponseTime;

					// TODO: populate global variables from parseresponse (is parseresponse only when the status code is ok?)
					bool hasFailures = false;
					if (response.StatusCode == testCase.VerifyResponseCode)
					{
						string content = response.Content;
						testResult.Success = true;

						// TODO: Parse the response into parsedresponses
						//ParseResponses(testCase.ParseResponses, content);

						// Verify positives
						List<RegexItem> failedPositives = GetFailedVerifications(testCase.VerifyPositives, content);

						// Verify Negatives
						List<RegexItem> failedNegatives = GetFailedVerifications(testCase.VerifyNegatives, content);
					}
					else
					{
						testResult.Message = testCase.ErrorMessage;
						testResult.Success = false;
					}

					// Log request (TODO: smarter way to log this earlier (why is it here not above?))
					if (_config.GlobalHttpLog == LogType.All || testCase.LogRequest)
					{
						// TODO: On fail support
						_logWriter.WriteRequest(testCase.Method, testCase.Url, testCase.AddHeader);
					}

					// Log response
					if (_config.GlobalHttpLog != LogType.None || testCase.LogResponse)
					{
						// TODO: On fail support
						_logWriter.WriteResponse(response.StatusCode, response.Headers, response.Content);
					}

					// TODO: sleep if sleep is set (Thread.Sleep?)
					_logWriter.WriteSeperator();
				}
			}

			return runSummary;
		}

		private static List<RegexItem> GetFailedVerifications(List<RegexItem> verifications, string content)
		{
			var failedItems = new List<RegexItem>();

			foreach (RegexItem verify in verifications)
			{
				string verifyRegex = verify.Regex;
				if (!Regex.IsMatch(content, verifyRegex))
				{
					failedItems.Add(verify);
					Console.WriteLine("Verification failed: {0}", verify);
				}
			}

			return failedItems;
		}
	}
}
