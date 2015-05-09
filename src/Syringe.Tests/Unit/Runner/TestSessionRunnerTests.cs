using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Logging;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;
using Syringe.Core.Runner;
using Syringe.Tests.Unit.StubsMocks;

namespace Syringe.Tests.Unit.Runner
{
	public class TestSessionRunnerTests
	{
		private HttpClientMock _httpClientMock;
		private ResultWriterStub _resultWriterStub;
		private HttpResponse _httpResponse;

		[SetUp]
		public void Setup()
		{
			Log.UseConsole();
		}

		[Test]
		public void Run_should_repeat_testcases_from_repeat_property()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case() { Url = "foo1" }, 
			});
			reader.CaseCollection.Repeat = 10;

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			Assert.That(session.TestCaseResults.Count, Is.EqualTo(10));
		}

		[Test]
		public void Run_should_set_MinResponseTime_and_MaxResponseTime_from_http_response_times()
		{
			// Arrange
			var config = new Config();

			var response = new HttpResponse();
			response.ResponseTime = TimeSpan.FromSeconds(5);

			HttpClientMock httpClient = new HttpClientMock(response);
			httpClient.ResponseTimes = new List<TimeSpan>()
			{
				// Deliberately mixed up order
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(88),
				TimeSpan.FromSeconds(3),
				TimeSpan.FromSeconds(10)
			};
			httpClient.Response = response;

			IResultWriter resultWriter = new ResultWriterStub();
			var runner = new TestSessionRunner(config, httpClient, resultWriter);

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case() { Url = "foo1" }, 
				new Case() { Url = "foo2" },
				new Case() { Url = "foo3" },
				new Case() { Url = "foo4" },
			});

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			Assert.That(session.MinResponseTime, Is.EqualTo(TimeSpan.FromSeconds(3)));
			Assert.That(session.MaxResponseTime, Is.EqualTo(TimeSpan.FromSeconds(88)));
		}

		[Test]
		public void Run_should_populate_StartTime_and_EndTime_and_TotalRunTime()
		{
			// Arrange
			var beforeStart = DateTime.UtcNow;
			var config = new Config();

			var response = new HttpResponse();
			response.ResponseTime = TimeSpan.FromSeconds(5);

			HttpClientMock httpClient = new HttpClientMock(response);
			IResultWriter resultWriter = new ResultWriterStub();
			var runner = new TestSessionRunner(config, httpClient, resultWriter);

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case() { Url = "foo1" }, 
			});

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			Assert.That(session.StartTime, Is.GreaterThanOrEqualTo(beforeStart));
			Assert.That(session.EndTime, Is.GreaterThanOrEqualTo(session.StartTime));
			Assert.That(session.TotalRunTime, Is.EqualTo(session.EndTime - session.StartTime));
		}

		[Test]
		public void Run_should_replace_variables_in_url()
		{
			// Arrange
			var config = new Config();
			config.BaseUrl = "http://www.yahoo.com";
			TestSessionRunner runner = CreateRunner(config);

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case() { Url = "{baseurl}/foo/test.html" }, 
			});

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			Assert.That(session.TestCaseResults[0].ActualUrl, Is.EqualTo("http://www.yahoo.com/foo/test.html"));
		}

		[Test]
		public void Run_should_set_parsed_variables()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);
			_httpClientMock.Response.Content = "some content";
			_httpClientMock.Response.StatusCode = HttpStatusCode.OK;

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case()
				{
					Url = "case1", 
					VerifyResponseCode = HttpStatusCode.OK,
					ParseResponses = new List<ParsedResponseItem>()
					{
						new ParsedResponseItem("1", "some content")
					},
					VerifyPositives = new List<VerificationItem>()
					{
						new VerificationItem("positive-1", "{parsedresponse1}", VerifyType.Positive)
					},
				}
			});

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			Assert.That(session.TestCaseResults[0].VerifyPositiveResults[0].Success, Is.True);
		}

		[Test]
		public void Run_should_set_parsedvariables_across_testcases()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);
			_httpClientMock.Responses = new List<HttpResponse>()
			{
				new HttpResponse()
				{
					StatusCode = HttpStatusCode.OK,
					Content = "1st content SECRET_KEY"
				},
				new HttpResponse()
				{
					StatusCode = HttpStatusCode.OK,
					Content = "2nd content - SECRET_KEY in here to match"
				},
				new HttpResponse()
				{
					StatusCode = HttpStatusCode.OK,
					Content = "3rd content - SECRET_KEY in here to match"
				}
			};

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case()
				{
					Url = "case1", 
					VerifyResponseCode = HttpStatusCode.OK,
					ParseResponses = new List<ParsedResponseItem>()
					{
						new ParsedResponseItem("1", @"(SECRET_KEY)")
					},
				}, 
				new Case()
				{
					Url = "case2", 
					VerifyResponseCode = HttpStatusCode.OK,
					ParseResponses = new List<ParsedResponseItem>()
					{
						new ParsedResponseItem("2", @"(SECRET_KEY)")
					},
					VerifyPositives = new List<VerificationItem>()
					{
						// Test the parsedresponse variable from the 1st case
						new VerificationItem("positive-for-case-2", "{parsedresponse1}", VerifyType.Positive)
					},
				}, 
				new Case()
				{
					Url = "case3", 
					VerifyResponseCode = HttpStatusCode.OK,
					VerifyPositives = new List<VerificationItem>()
					{
						// Test the parsedresponse variable from the 1st case
						new VerificationItem("positive-for-case-3", "{parsedresponse2}", VerifyType.Positive)
					},
				}
			});

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			Assert.That(session.TestCaseResults[1].VerifyPositiveResults[0].Success, Is.True);
			Assert.That(session.TestCaseResults[2].VerifyPositiveResults[0].Success, Is.True);
		}

		[Test]
		public void Run_should_set_testresult_success_and_response_when_httpcode_passes()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);
			_httpClientMock.Response.StatusCode = HttpStatusCode.OK;

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case()
				{
					Url = "foo1", 
					VerifyResponseCode = HttpStatusCode.OK
				}, 
			});

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			Assert.That(session.TestCaseResults[0].Success, Is.True);
			Assert.That(session.TestCaseResults[0].HttpResponse, Is.EqualTo(_httpClientMock.Response));
		}

		[Test]
		public void Run_should_set_testresult_success_and_response_when_httpcode_fails()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);
			_httpClientMock.Response.StatusCode = HttpStatusCode.OK;

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case()
				{
					Url = "foo1", 
					VerifyResponseCode = HttpStatusCode.Ambiguous
				}, 
			});

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			Assert.That(session.TestCaseResults[0].Success, Is.False);
			Assert.That(session.TestCaseResults[0].HttpResponse, Is.EqualTo(_httpClientMock.Response));
		}

		[Test]
		public void Run_should_set_message_from_case_errormessage_when_httpcode_fails()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case() { Url = "foo1", ErrorMessage = "It broke", VerifyResponseCode = HttpStatusCode.Ambiguous}, 
			});

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			Assert.That(session.TestCaseResults[0].Message, Is.EqualTo("It broke"));
		}


		[Test]
		public void Run_should_write_test_result_to_resultwriter()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case() { Url = "foo1"}, 
			});

			// Act
			runner.Run(reader);

			// Assert
			Assert.That(_resultWriterStub.StringBuilder.ToString(), Is.Not.Empty.Or.Null);
		}

		[Test]
		public void Run_should_sleep_thread_in_seconds_when_case_has_sleep_set()
		{
			// Arrange
			int seconds = 2;
			var now = DateTime.UtcNow;
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case() { Url = "foo1", Sleep = seconds }
			});

			// Act
			runner.Run(reader);
			var timeAfterRun = DateTime.UtcNow;

			// Assert
			Assert.That(timeAfterRun, Is.GreaterThanOrEqualTo(now.AddSeconds(seconds)));
		}

		[Test]
		public void Run_should_log_request_and_responses_using_httpclient_when_logging_is_enabled()
		{
			// Arrange
			var config = new Config();
			config.GlobalHttpLog = LogType.All;

			TestSessionRunner runner = CreateRunner(config);

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case() { Url = "foo1" } 
			});

			// Act
			runner.Run(reader);

			// Assert
			Assert.That(_httpClientMock.LogLastRequestCalled, Is.True);
			Assert.That(_httpClientMock.LogLastResponseCalled, Is.True);
		}

		[Test]
		public void Run_should_verify_positive_and_negative_items_when_httpcode_passes()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);
			_httpClientMock.Response.StatusCode = HttpStatusCode.OK;
			_httpClientMock.Response.Content = "some content";

			var reader = new TestCaseReaderMock();
			reader.CaseCollection = CreateCaseCollection(new[] 
			{
				new Case()
				{
					Url = "foo1", 
					VerifyResponseCode = HttpStatusCode.OK,
					VerifyPositives = new List<VerificationItem>()
					{
						new VerificationItem("positive-1", "some content", VerifyType.Positive)
					},
					VerifyNegatives = new List<VerificationItem>()
					{
						new VerificationItem("negative-1", "no text like this", VerifyType.Negative)
					}
				}, 
			});

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			Assert.That(session.TestCaseResults[0].Success, Is.True);
			Assert.That(session.TestCaseResults[0].VerifyPositiveResults.Count, Is.EqualTo(1));
			Assert.That(session.TestCaseResults[0].VerifyPositiveResults[0].Success, Is.True);

			Assert.That(session.TestCaseResults[0].VerifyNegativeResults.Count, Is.EqualTo(1));
			Assert.That(session.TestCaseResults[0].VerifyNegativeResults[0].Success, Is.True);
		}

		[Test]
		public void ShouldLogRequest_should_return_true_when_case_fails_and_onfail()
		{
			// Arrange
			var config = new Config();
			config.GlobalHttpLog = LogType.OnFail;
			TestSessionRunner runner = CreateRunner(config);

			var testResult = new TestCaseResult() { ResponseCodeSuccess = false };
			var testCase = new Case();

			// Act
			bool shouldLog = runner.ShouldLogRequest(testResult, testCase);

			// Assert
			Assert.That(shouldLog, Is.True);
		}

		[Test]
		public void ShouldLogRequest_should_return_true_when_logtype_is_all()
		{
			// Arrange
			var config = new Config();
			config.GlobalHttpLog = LogType.All;
			TestSessionRunner runner = CreateRunner(config);

			var testResult = new TestCaseResult() {ResponseCodeSuccess = true };
			var testCase = new Case();

			// Act
			bool shouldLog = runner.ShouldLogRequest(testResult, testCase);

			// Assert
			Assert.That(shouldLog, Is.True);
		}

		[Test]
		public void ShouldLogRequest_should_return_true_when_testcase_logrequest_is_true()
		{
			// Arrange
			var config = new Config();
			config.GlobalHttpLog = LogType.None;
			TestSessionRunner runner = CreateRunner(config);

			var testResult = new TestCaseResult();
			var testCase = new Case() {LogRequest = true};

			// Act
			bool shouldLog = runner.ShouldLogRequest(testResult, testCase);

			// Assert
			Assert.That(shouldLog, Is.True);
		}

		[Test]
		public void ShouldLogResponse_should_return_true_when_case_fails_and_onfail()
		{
			// Arrange
			var config = new Config();
			config.GlobalHttpLog = LogType.OnFail;
			TestSessionRunner runner = CreateRunner(config);

			var testResult = new TestCaseResult() { ResponseCodeSuccess = false };
			var testCase = new Case();

			// Act
			bool shouldLog = runner.ShouldLogResponse(testResult, testCase);

			// Assert
			Assert.That(shouldLog, Is.True);
		}

		[Test]
		public void ShouldLogResponse_should_return_true_when_logtype_is_all()
		{
			// Arrange
			var config = new Config();
			config.GlobalHttpLog = LogType.All;
			TestSessionRunner runner = CreateRunner(config);

			var testResult = new TestCaseResult() { ResponseCodeSuccess = true };
			var testCase = new Case();

			// Act
			bool shouldLog = runner.ShouldLogResponse(testResult, testCase);

			// Assert
			Assert.That(shouldLog, Is.True);
		}

		[Test]
		public void ShouldLogResponse_should_return_true_when_testcase_logresponse_is_true()
		{
			// Arrange
			var config = new Config();
			config.GlobalHttpLog = LogType.None;
			TestSessionRunner runner = CreateRunner(config);

			var testResult = new TestCaseResult();
			var testCase = new Case() { LogResponse = true };

			// Act
			bool shouldLog = runner.ShouldLogResponse(testResult, testCase);

			// Assert
			Assert.That(shouldLog, Is.True);
		}

		private TestSessionRunner CreateRunner(Config config)
		{
			_httpResponse = new HttpResponse();
			_httpClientMock = new HttpClientMock(_httpResponse);
			_resultWriterStub = new ResultWriterStub();

			return new TestSessionRunner(config, _httpClientMock, _resultWriterStub);
		}

		private CaseCollection CreateCaseCollection(Case[] cases)
		{
			var testCases = new List<Case>();
			testCases.AddRange(cases);

			var collection = new CaseCollection();
			collection.TestCases = testCases;

			return collection;
		}
	}
}
