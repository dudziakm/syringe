using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using RestSharp;
using RestSharp.Contrib;
using Syringe.Core.Http;
using Syringe.Core.Xml;
using Syringe.Core.Xml.LegacyConverter;
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

		public TestCaseRunner(Config config, IHttpClient httpClient,  IHttpLogWriter logWriter)
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
				var testCaseReader = new TestCaseReader();
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
					    ParseResponses(testCase.ParseResponses, content);

                        // Verify positives
                        List<NumberedAttribute> failedPositives = GetFailedVerifications(testCase.VerifyPositives, content);

                        // Verify Negatives
                        List<NumberedAttribute> failedNegatives = GetFailedVerifications(testCase.VerifyNegatives, content);
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


        // Move to XML
	    private Dictionary<int, string> ParseResponses(List<NumberedAttribute> parseResponses, string content)
	    {
	        var responseVariables = new Dictionary<int, string>();

            // Parse a string from the HTTP response for use in subsequent requests. This is mostly used for passing Session ID's, 
            // but can be applied to any case where you need to pass a dynamically generated value. It takes the arguments in the format 
            // "leftboundary|rightboundary", and an optional third argument "leftboundary|rightboundary|escape" when you want to 
            // force escaping of all non-alphanumeric characters

            // Example:
            //      <input type="hidden" name="__VIEWSTATE" value="dDwtMTA4NzczMzUxMjs7Ps1HmLfiYGewI+2JaAxhcpiCtj52" />
            //
            //      parseresponse='__VIEWSTATE" value="|"|escape'
            //
            // This is then used by:
            //      postbody="value=123&__VIEWSTATE={PARSEDRESULT}"
            //

	        foreach (NumberedAttribute item in parseResponses)
	        {
	            string parsedResponse = item.Value;
	            if (parsedResponse.Contains("|"))
	            {
	                string[] parts = parsedResponse.Split('|');
	                string remainingContent = "";

	                if (parts.Length > 1)
	                {
	                    int startIndex = content.IndexOf(parts[0]);

	                    if (startIndex > -1)
	                    {
	                        remainingContent = content.Substring(startIndex);

	                        if (parts.Length == 3)
	                        {
	                            remainingContent = HttpUtility.UrlEncode(remainingContent);
	                        }
	                    }
	                }

                    responseVariables.Add(item.Index, remainingContent);
	            }
	            else
	            {
                    // Is this right - if there's no pipe should it hold everything or nothing?
                    responseVariables.Add(item.Index, "");
	            }
	        }

	        return responseVariables;
	    }

	    private static List<NumberedAttribute> GetFailedVerifications(List<NumberedAttribute> verifications, string content)
	    {
            var failedItems = new List<NumberedAttribute>();

            foreach (NumberedAttribute verify in verifications)
            {
                string verifyRegex = verify.Value;
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
