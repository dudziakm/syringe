using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;
using Syringe.Core.Runner;
using Syringe.Tests.Unit.StubsMocks;

namespace Syringe.Tests.Unit.Runner
{
	public class TestSessionRunnerTests
	{
		private CaseCollection CreateCaseCollection(Case[] cases)
		{
			var testCases = new List<Case>();
			testCases.AddRange(cases);

			var collection = new CaseCollection();
			collection.TestCases = testCases;

			return collection;
		}

		[Test]
		public void Run_should_set_MinResponseTime_and_MaxResponseTime_from_http_response_times()
		{
			// Arrange
			var config = new Config();

			var response = new HttpResponse();
			response.ResponseTime = TimeSpan.FromSeconds(5);

			HttpClientMock httpClient = new HttpClientMock();
			httpClient.ResponseTimes = new List<TimeSpan>()
			{
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
			DateTime now = DateTime.UtcNow;
			var config = new Config();

			var response = new HttpResponse();
			response.ResponseTime = TimeSpan.FromSeconds(5);

			HttpClientMock httpClient = new HttpClientMock();
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
			Assert.That(session.StartTime, Is.GreaterThanOrEqualTo(now));
			Assert.That(session.EndTime, Is.GreaterThanOrEqualTo(now));
			Assert.That(session.TotalRunTime, Is.EqualTo(session.EndTime - session.StartTime));
		}

		[Test]
		public void ShouldLogRequest_should_do_something()
		{
			// Arrange
			var config = new Config();

			var response = new HttpResponse();
			HttpClientMock httpClient = new HttpClientMock();
			IResultWriter resultWriter = new ResultWriterStub();

			var runner = new TestSessionRunner(config, httpClient, resultWriter);
			var reader = new TestCaseReaderMock();

			// Act
			var testCase = new Case();
			var variables = new SessionVariables();
			var verificationMatcher = new VerificationsMatcher(variables);
			runner.RunCase(testCase, variables, verificationMatcher);

			// Assert
		}
	}
}
