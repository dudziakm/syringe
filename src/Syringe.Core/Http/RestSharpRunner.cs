using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core.Xml;

namespace Syringe.Core.Http
{
	public class RestSharpRunner
	{
		// Turn into interface
		// bool addTestCase
		// TODO: check for global
		// TODO: check per case
		// TODO: check onfail

		// TODO: logger for cases output (StdOut,Xml etc.)
		// TODO: config.xml takes <testcases>

		private readonly Config _config;
		private readonly IHttpLogWriter _logWriter;
		private readonly CookieContainer _cookieContainer;

		public RestSharpRunner(Config config, IHttpLogWriter logWriter)
		{
			_config = config;
			_logWriter = logWriter;
			_cookieContainer = new CookieContainer();
		}

		public void Run(string testCaseFilename)
		{
			bool logRequests = _config.GlobalHttpLog;
			bool logResponses = _config.GlobalHttpLog;

			using (var stringReader = new StringReader(File.ReadAllText(testCaseFilename)))
			{
				var testCaseReader = new TestCaseReader();
				TestCaseContainer testCaseContainer = testCaseReader.Read(stringReader);

				foreach (TestCase testCase in testCaseContainer.TestCases)
				{
					// TODO: variables on URL

					var client = new RestClient(testCase.Url); 
					client.CookieContainer = _cookieContainer;

					Method method = GetMethodEnum(testCase);
					var request = new RestRequest(method);
					if (method == Method.POST)
					{
						request.AddParameter(testCase.PostType, testCase.PostBody, ParameterType.RequestBody);
					}

					foreach (var keyValuePair in testCase.AddHeader) // TODO: change property name to Headers ?
					{
						request.AddHeader(keyValuePair.Key, keyValuePair.Value);
					}

					// TODO: Log the HTTP request to memory, unless the testcase has turned it off

					// TODO: Add a new TestCaseResult to the collection

					// TODO: Log the HTTP response to memory, unless the testcase has turned it off
					IRestResponse response = client.Execute(request);

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

						// TODO: 

						// TODO: Do lots of verify positive, verifynegative (regexes)

					}
					else
					{
						// TODO: Set the Testcaseresult.errormessage to equal the testcase error message
						// TODO: Set the Testcaseresult.haserrors to true
					}

					// TODO: sleep if sleep is set (Thread.Sleep?)
				}
			}
		}

		private Method GetMethodEnum(TestCase testCase)
		{
			var method = Method.GET;

			if (Enum.IsDefined(typeof(Method), testCase.Method))
			{
				Enum.TryParse(testCase.Method, out method);
			}

			return method;
		}
	}
}
