using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using RestSharp;
using Syringe.Core.Http;
using Syringe.Core.Xml;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Core
{
	public class TestCaseRunner
	{
		// TODO: variables on URL
		// TODO: logger for cases output (StdOut,Xml etc.)
		// TODO: config.xml takes <testcases>

		private readonly Config _config;
		private readonly IHttpClient _httpClient;
		private readonly IHttpLogWriter _logWriter;

		public TestCaseRunner(Config config, IHttpClient httpClient,  IHttpLogWriter logWriter)
		{
			_config = config;
			_httpClient = httpClient;
			_logWriter = logWriter;
		}

		public void Run(string testCaseFilename)
		{
			using (var stringReader = new StringReader(File.ReadAllText(testCaseFilename)))
			{
				var testCaseReader = new TestCaseReader();
				TestCaseContainer testCaseContainer = testCaseReader.Read(stringReader);

				foreach (TestCase testCase in testCaseContainer.TestCases)
				{
					// TODO: Add a new TestCaseResult to the collection

					HttpResponse response = _httpClient.MakeRequest(testCase.Method, testCase.Url, testCase.PostType, testCase.PostBody, testCase.AddHeader);

					// TODO: populate global variables from parseresponse (is parseresponse only when the status code is ok?)

					if (response.StatusCode == testCase.VerifyResponseCode)
					{
						string content = response.Content;
						int counter = 0;
						var failedPositives = new List<int>();
						foreach (string verifyPositive in testCase.VerifyPositives)
						{
							if (!Regex.IsMatch(content, verifyPositive))
							{
								failedPositives.Add(counter);
								Console.WriteLine("Couldn't find positive: {0}", verifyPositive);
							}

							counter++;
						}

						// TODO: Parse the response into parsedresponses

						// TODO: Do lots of verify positive, verifynegative (regexes)

					}
					else
					{
						// TODO: Set the Testcaseresult.errormessage to equal the testcase error message
						// TODO: Set the Testcaseresult.haserrors to true
					}

					// Log request
					if (_config.GlobalHttpLog == LogType.All || testCase.LogRequest)
					{
						// TODO: On fail support (or a smarter way to log this earlier)
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
		}
	}
}
