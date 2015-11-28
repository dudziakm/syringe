using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Syringe.Core.Http;
using Syringe.Core.Repositories;
using Syringe.Core.Results;
using Syringe.Core.Runner;
using Syringe.Core.TestCases;
using Syringe.Core.TestCases.Configuration;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Unit.Runner
{
	public class TestSessionRunnerTests
	{
		private HttpClientMock _httpClientMock;
		private HttpRequestInfo _httpRequestInfo;

		[SetUp]
		public void Setup()
		{
			TestHelpers.EnableLogging();
		}

		private ITestCaseSessionRepository GetRepository()
		{
			return new TestCaseSessionRepositoryMock();
		}

		[Test]
		public async Task Run_should_repeat_testcases_from_repeat_property()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1" },
			});
			caseCollection.Repeat = 10;

			// Act
			TestCaseSession session = await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(session.TestCaseResults.Count(), Is.EqualTo(10));
		}

		[Test]
		public async Task Run_should_set_MinResponseTime_and_MaxResponseTime_from_http_response_times()
		{
			// Arrange
			var config = new Config();

			var response = new HttpRequestInfo();
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
			httpClient.RequestInfo = response;

			var runner = new TestSessionRunner(config, httpClient, GetRepository());

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1" },
				new Case() { Url = "foo2" },
				new Case() { Url = "foo3" },
				new Case() { Url = "foo4" },
			});

			// Act
			TestCaseSession session = await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(session.MinResponseTime, Is.EqualTo(TimeSpan.FromSeconds(3)));
			Assert.That(session.MaxResponseTime, Is.EqualTo(TimeSpan.FromSeconds(88)));
		}

		[Test]
		public async Task Run_should_populate_StartTime_and_EndTime_and_TotalRunTime()
		{
			// Arrange
			var beforeStart = DateTime.UtcNow;
			var config = new Config();

			var requestInfo = new HttpRequestInfo();
			requestInfo.ResponseTime = TimeSpan.FromSeconds(5);

			HttpClientMock httpClient = new HttpClientMock(requestInfo);
			var runner = new TestSessionRunner(config, httpClient, GetRepository());

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1" },
			});

			// Act
			TestCaseSession session = await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(session.StartTime, Is.GreaterThanOrEqualTo(beforeStart));
			Assert.That(session.EndTime, Is.GreaterThanOrEqualTo(session.StartTime));
			Assert.That(session.TotalRunTime, Is.EqualTo(session.EndTime - session.StartTime));
		}

		[Test]
		public async Task Run_should_replace_variables_in_url()
		{
			// Arrange
			var config = new Config();
			config.BaseUrl = "http://www.yahoo.com";
			TestSessionRunner runner = CreateRunner(config);

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "{baseurl}/foo/test.html" },
			});

			// Act
			TestCaseSession session = await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(session.TestCaseResults.Single().ActualUrl, Is.EqualTo("http://www.yahoo.com/foo/test.html"));
		}

		[Test]
		public async Task Run_should_set_parsed_variables()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);
			_httpClientMock.RequestInfo.Response.Content = "some content";
			_httpClientMock.RequestInfo.Response.StatusCode = HttpStatusCode.OK;

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case()
				{
					Url = "case1",
					VerifyResponseCode = HttpStatusCode.OK,
					ParseResponses = new List<ParseResponseItem>()
					{
						new ParseResponseItem("1", "some content")
					},
					VerifyPositives = new List<VerificationItem>()
					{
						new VerificationItem("positive-1", "{parsedresponse1}", VerifyType.Positive)
					},
				}
			});

			// Act
			TestCaseSession session = await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(session.TestCaseResults.Single().VerifyPositiveResults[0].Success, Is.True);
		}

		[Test]
		public async Task Run_should_set_parseresponsevariables_across_testcases()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);
			_httpClientMock.Responses = new List<HttpRequestInfo>()
			{
				new HttpRequestInfo()
				{
					Response = new RestResponseStub()
					{
						StatusCode = HttpStatusCode.OK,
						Content = "1st content SECRET_KEY"
					}
				},
				new HttpRequestInfo()
				{
					Response = new RestResponseStub()
					{
						StatusCode = HttpStatusCode.OK,
						Content = "2nd content - SECRET_KEY in here to match"
					}
				},
				new HttpRequestInfo()
				{
					Response = new RestResponseStub()
					{
						StatusCode = HttpStatusCode.OK,
						Content = "3rd content - SECRET_KEY in here to match"
					}
				}
			};

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case()
				{
					Url = "case1",
					VerifyResponseCode = HttpStatusCode.OK,
					ParseResponses = new List<ParseResponseItem>()
					{
						new ParseResponseItem("1", @"(SECRET_KEY)")
					},
				},
				new Case()
				{
					Url = "case2",
					VerifyResponseCode = HttpStatusCode.OK,
					ParseResponses = new List<ParseResponseItem>()
					{
						new ParseResponseItem("2", @"(SECRET_KEY)")
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
						// Test the parseresponse variable from the 1st case
						new VerificationItem("positive-for-case-3", "{parsedresponse2}", VerifyType.Positive)
					},
				}
			});

			// Act
			TestCaseSession session = await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(session.TestCaseResults.ElementAt(1).VerifyPositiveResults[0].Success, Is.True);
			Assert.That(session.TestCaseResults.ElementAt(2).VerifyPositiveResults[0].Success, Is.True);
		}

		[Test]
		public async Task Run_should_set_testresult_success_and_response_when_httpcode_passes()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);
			_httpClientMock.RequestInfo.Response.StatusCode = HttpStatusCode.OK;

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case()
				{
					Url = "foo1",
					VerifyResponseCode = HttpStatusCode.OK
				},
			});

			// Act
			TestCaseSession session = await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(session.TestCaseResults.Single().Success, Is.True);
			Assert.That(session.TestCaseResults.Single().HttpRequestInfo, Is.EqualTo(_httpClientMock.RequestInfo));
		}

		[Test]
		public async Task Run_should_set_testresult_success_and_response_when_httpcode_fails()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);
			_httpClientMock.RequestInfo.Response = new RestResponseStub()
			{
				StatusCode = HttpStatusCode.OK
			};

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case()
				{
					Url = "foo1",
					VerifyResponseCode = HttpStatusCode.Ambiguous
				},
			});

			// Act
			TestCaseSession session = await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(session.TestCaseResults.Single().Success, Is.False);
			Assert.That(session.TestCaseResults.Single().HttpRequestInfo, Is.EqualTo(_httpClientMock.RequestInfo));
		}

		[Test]
		public async Task Run_should_set_message_from_case_errormessage_when_httpcode_fails()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1", ErrorMessage = "It broke", VerifyResponseCode = HttpStatusCode.Ambiguous},
			});

			// Act
			TestCaseSession session = await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(session.TestCaseResults.Single().Message, Is.EqualTo("It broke"));
		}


		[Test]
		public async Task Run_should_save_testcasesession_to_repository()
		{
			// Arrange
			var config = new Config();
			var repository = new TestCaseSessionRepositoryMock();

			TestSessionRunner runner = CreateRunner(config);
			runner.Repository = repository;

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1"},
			});

			// Act
			await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(repository.SavedSession, Is.Not.Null);
			Assert.That(repository.SavedSession.TestCaseResults.Count(), Is.EqualTo(1));
		}

		[Test]
		public async Task Run_should_sleep_thread_in_seconds_when_case_has_sleep_set()
		{
			// Arrange
			int seconds = 2;
			var now = DateTime.UtcNow;
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1", Sleep = seconds }
			});

			// Act
			await runner.RunAsync(caseCollection);
			var timeAfterRun = DateTime.UtcNow;

			// Assert
			Assert.That(timeAfterRun, Is.GreaterThanOrEqualTo(now.AddSeconds(seconds)));
		}

		[Test]
		public async Task Run_should_verify_positive_and_negative_items_when_httpcode_passes()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);
			_httpClientMock.RequestInfo.Response = new RestResponseStub()
			{
				StatusCode = HttpStatusCode.OK,
				Content = "some content"
			};

			var caseCollection = CreateCaseCollection(new[]
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
			TestCaseSession session = await runner.RunAsync(caseCollection);

			// Assert
			var result = session.TestCaseResults.Single();
			Assert.That(result.Success, Is.True);
			Assert.That(result.VerifyPositiveResults.Count, Is.EqualTo(1));
			Assert.That(result.VerifyPositiveResults[0].Success, Is.True);

			Assert.That(result.VerifyNegativeResults.Count, Is.EqualTo(1));
			Assert.That(result.VerifyNegativeResults[0].Success, Is.True);
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

			var testResult = new TestCaseResult() { ResponseCodeSuccess = true };
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
			var testCase = new Case() { LogRequest = true };

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

		[Test]
		public async Task Run_should_notify_observers_of_existing_results()
		{
			// Arrange
			var observedResults = new List<TestCaseResult>();

			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1" },
				new Case() { Url = "foo2" },
				new Case() { Url = "foo3" }
			});

			await runner.RunAsync(caseCollection);

			// Act
			runner.Subscribe(r => { observedResults.Add(r); });

			// Assert
			Assert.That(observedResults.Select(r => r.ActualUrl), Is.EquivalentTo(new[] { "foo1", "foo2", "foo3" }), "Should have observed all of the results.");
		}

		[Test]
		public async Task Run_should_notify_observers_of_new_results()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1" },
				new Case() { Url = "foo2" },
				new Case() { Url = "foo3" }
			});

			var observedResults = new List<TestCaseResult>();

			// Act
			runner.Subscribe(r => { observedResults.Add(r); });

			await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(observedResults.Select(r => r.ActualUrl), Is.EquivalentTo(new[] { "foo1", "foo2", "foo3" }), "Should have observed all of the results.");
		}

		[Test]
		public async Task Run_should_not_notify_disposed_observers_of_new_results()
		{
			// Arrange
			var config = new Config();
			var httpClientMock = new Mock<IHttpClient>();

			IDisposable subscription = null;

			httpClientMock
				.Setup(
					c =>
						c.ExecuteRequestAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
							It.IsAny<IEnumerable<HeaderItem>>()))
				.Returns(Task.FromResult(new HttpRequestInfo()));

			// Dispose of the subscription before processing the third request.
			httpClientMock
				.Setup(c => c.ExecuteRequestAsync(It.IsAny<string>(), "foo3", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<HeaderItem>>()))
				.Callback(() => { if (subscription != null) subscription.Dispose(); })
				.Returns(Task.FromResult(new HttpRequestInfo()));

			TestSessionRunner runner = new TestSessionRunner(config, httpClientMock.Object, GetRepository());

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1" },
				new Case() { Url = "foo2" },
				new Case() { Url = "foo3" }
			});

			var observedResults = new List<TestCaseResult>();

			// Act
			subscription = runner.Subscribe(r => { observedResults.Add(r); });

			await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(observedResults.Select(r => r.ActualUrl), Is.EquivalentTo(new[] { "foo1", "foo2" }), "Should not have included the result after having been disposed.");
		}

		[Test]
		public async Task Run_should_notify_subscribers_of_completion_when_test_case_session_ends()
		{
			// Arrange
			var config = new Config();
			TestSessionRunner runner = CreateRunner(config);

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1" },
				new Case() { Url = "foo2" },
				new Case() { Url = "foo3" }
			});

			var completed = false;

			runner.Subscribe(r => { }, onCompleted: () => completed = true);

			Assume.That(completed, Is.False);

			// Act
			await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(completed, Is.True, "Should have notified of completion.");
		}

		[Test]
		public async Task Run_should_notify_subscribers_of_result_on_error()
		{
			// Arrange
			var httpClientMock = new Mock<IHttpClient>();

			// Throw an error.
			httpClientMock
				.Setup(c => c.ExecuteRequestAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<HeaderItem>>()))
				.Throws(new InvalidOperationException("Bad"));

			TestSessionRunner runner = new TestSessionRunner(new Config(), httpClientMock.Object, GetRepository());

			var caseCollection = CreateCaseCollection(new[]
			{
				new Case() { Url = "foo1" },
				new Case() { Url = "foo2" },
				new Case() { Url = "foo3" }
			});

			TestCaseResult capturedResult = null;
			runner.Subscribe(r => capturedResult = r);

			// Act
			await runner.RunAsync(caseCollection);

			// Assert
			Assert.That(capturedResult, Is.Not.Null, "Should have notified of the result.");
			Assert.That(capturedResult.Success, Is.False, "Should not have succeeded.");
		}

		private TestSessionRunner CreateRunner(Config config)
		{
			_httpRequestInfo = new HttpRequestInfo();
			_httpClientMock = new HttpClientMock(_httpRequestInfo);
			_httpClientMock.RequestInfo = new HttpRequestInfo()
			{
				Response = new RestResponseStub(),
				Request = new RestRequestStub()
			};

			return new TestSessionRunner(config, _httpClientMock, GetRepository());
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
