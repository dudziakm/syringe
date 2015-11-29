﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RestSharp;
using Syringe.Core.Repositories.MongoDB;
using Syringe.Core.Results;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Integration.Repository.MongoDB
{
	public class TestCaseSessionRepositoryTests
	{
		private TestCaseSessionRepository CreateTestCaseSessionRepository()
		{
			return new TestCaseSessionRepository(new Configuration() { DatabaseName = "Syringe-Tests"});	
		}

		[SetUp]
		public void SetUp()
		{
			CreateTestCaseSessionRepository().Wipe();
		}

		private static Fixture CreateFixture()
		{
			var fixture = new Fixture();
			fixture.Register<IRestResponse>(() => new RestResponse());
			fixture.Register<IRestRequest>(() => new RestRequest());
			return fixture;
		}

		[Test]
		public async Task Add_should_save_session()
		{
			// Arrange
			Fixture fixture = CreateFixture();
			var session = fixture.Create<TestCaseSession>();

			TestCaseSessionRepository repository = CreateTestCaseSessionRepository();

			// Act
			await repository.AddAsync(session);

			// Assert
			IEnumerable<SessionInfo> summaries = repository.GetSummaries();
			Assert.That(summaries.Count(), Is.EqualTo(1));
		}

		[Test]
		public async Task Delete_should_remove_the_session()
		{
			// Arrange
			Fixture fixture = CreateFixture();
			var session = fixture.Create<TestCaseSession>();

			TestCaseSessionRepository repository = CreateTestCaseSessionRepository();
			await repository.AddAsync(session);

			// Act
			await repository.DeleteAsync(session.Id);

			// Assert
			IEnumerable<SessionInfo> summaries = repository.GetSummaries();
			Assert.That(summaries.Count(), Is.EqualTo(0));
		}

		[Test]
		public async Task GetById_should_return_session()
		{
			// Arrange
			Fixture fixture = CreateFixture();
			var expectedSession = fixture.Create<TestCaseSession>();

			TestCaseSessionRepository repository = CreateTestCaseSessionRepository();
			await repository.AddAsync(expectedSession);

			// Act
			TestCaseSession actualSession = repository.GetById(expectedSession.Id);

			// Assert
			Assert.That(actualSession, Is.Not.Null, "couldn't find the session");
			string actual = GetAsJson(actualSession);
			string expected = GetAsJson(expectedSession);

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public async Task GetSummaries_should_return_sessioninfos()
		{
			// Arrange
			Fixture fixture = CreateFixture();
			var session1 = fixture.Create<TestCaseSession>();
			var session2 = fixture.Create<TestCaseSession>();

			TestCaseSessionRepository repository = CreateTestCaseSessionRepository();
			await repository.AddAsync(session1);
			await repository.AddAsync(session2);

			// Act
			IEnumerable<SessionInfo> summaries = repository.GetSummaries();

			// Assert
			Assert.That(summaries.Count(), Is.EqualTo(2));

			IEnumerable<Guid> ids = summaries.Select(x => x.Id);
			Assert.That(ids, Contains.Item(session1.Id));
			Assert.That(ids, Contains.Item(session2.Id));
		}

		[Test]
		public async Task GetSummariesForToday_should_return_sessioninfo_objects_for_today_only()
		{
			// Arrange
			Fixture fixture = CreateFixture();
			var todaySession1 = fixture.Create<TestCaseSession>();
			var todaySession2 = fixture.Create<TestCaseSession>();
			var otherSession1 = fixture.Create<TestCaseSession>();
			var otherSession2 = fixture.Create<TestCaseSession>();

			todaySession1.StartTime = DateTime.Today;
			todaySession1.EndTime = todaySession1.StartTime.AddMinutes(5);

			todaySession2.StartTime = DateTime.Today.AddHours(1);
			todaySession2.EndTime = todaySession2.StartTime.AddMinutes(5);

			otherSession1.StartTime = DateTime.Today.AddDays(-2);
			otherSession1.EndTime = otherSession1.StartTime.AddMinutes(10);

			otherSession2.StartTime = DateTime.Today.AddDays(-2);
			otherSession2.EndTime = otherSession2.StartTime.AddMinutes(10);

			TestCaseSessionRepository repository = CreateTestCaseSessionRepository();
			await repository.AddAsync(todaySession1);
			await repository.AddAsync(todaySession2);
			await repository.AddAsync(otherSession1);
			await repository.AddAsync(otherSession2);

			// Act
			IEnumerable<SessionInfo> summaries = repository.GetSummariesForToday();

			// Assert
			Assert.That(summaries.Count(), Is.EqualTo(2));

			IEnumerable<Guid> ids = summaries.Select(x => x.Id);
			Assert.That(ids, Contains.Item(todaySession1.Id));
			Assert.That(ids, Contains.Item(todaySession2.Id));
		}

		// JSON.NET customisations

		private string GetAsJson(object o)
		{
			return JsonConvert.SerializeObject(o, Formatting.Indented, new JsonSerializerSettings
			{
				// Stop JSON.NET from serializing getter properties
				ContractResolver = new WritablePropertiesOnlyResolver(),

				// Stop JSON.NET putting dates as UTC, as the Z breaks the asserts
				Converters = new List<JsonConverter>() { new JavaScriptDateTimeConverter() }
			});
		}

		private class WritablePropertiesOnlyResolver : DefaultContractResolver
		{
			protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
			{
				IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
				return props.Where(p => p.Writable).ToList();
			}
		}
	}
}