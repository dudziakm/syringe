using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Syringe.Core.Http;
using Syringe.Core.Repositories.MongoDB;
using Syringe.Core.Results;

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
			SetUpFixture.WaitForDatabaseWipe();
		}

		[Test]
		public void Add_should_save_session()
		{
			// Arrange
			var fixture = new Fixture();
			var session = fixture.Create<TestCaseSession>();

			TestCaseSessionRepository repository = CreateTestCaseSessionRepository();

			// Act
			repository.Add(session);

			// Assert
			IEnumerable<SessionInfo> summaries = repository.GetSummaries();
			Assert.That(summaries.Count(), Is.EqualTo(1));
		}

		[Test]
		public void Delete_should_remove_the_session()
		{
			// Arrange
			var fixture = new Fixture();
			var session = fixture.Create<TestCaseSession>();

			TestCaseSessionRepository repository = CreateTestCaseSessionRepository();
			repository.Add(session);

			// Act
			repository.Delete(session);

			// Assert
			IEnumerable<SessionInfo> summaries = repository.GetSummaries();
			Assert.That(summaries.Count(), Is.EqualTo(0));
		}

		[Test]
		public void GetById_should_return_session()
		{
			// Arrange
			var fixture = new Fixture();
			var expectedSession = fixture.Create<TestCaseSession>();

			TestCaseSessionRepository repository = CreateTestCaseSessionRepository();
			repository.Add(expectedSession);

			// Act
			TestCaseSession actualSession = repository.GetById(expectedSession.Id);

			// Assert
			Assert.That(actualSession, Is.Not.Null, "couldn't find the session");
			string actual = GetAsJson(actualSession);
			string expected = GetAsJson(expectedSession);

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void GetSummaries_should_return_sessioninfos()
		{
			// Arrange
			var fixture = new Fixture();
			var session1 = fixture.Create<TestCaseSession>();
			var session2 = fixture.Create<TestCaseSession>();

			TestCaseSessionRepository repository = CreateTestCaseSessionRepository();
			repository.Add(session1);
			repository.Add(session2);
			Thread.Sleep(100);

			// Act
			IEnumerable<SessionInfo> summaries = repository.GetSummaries();

			// Assert
			Assert.That(summaries.Count(), Is.EqualTo(2));

			IEnumerable<Guid> ids = summaries.Select(x => x.Id);
			Assert.That(ids, Contains.Item(session1.Id));
			Assert.That(ids, Contains.Item(session2.Id));
		}

		[Test]
		public void GetSummariesForToday_should_return_sessioninfo_objects_for_today_only()
		{
			// Arrange
			var fixture = new Fixture();
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
			repository.Add(todaySession1);
			repository.Add(todaySession2);
			repository.Add(otherSession1);
			repository.Add(otherSession2);

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