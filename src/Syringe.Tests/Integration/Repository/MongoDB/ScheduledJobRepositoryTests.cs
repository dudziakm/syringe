using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Core.Repositories.MongoDB;
using Syringe.Core.Schedule;

namespace Syringe.Tests.Integration.Repository.MongoDB
{
	public class ScheduledJobRepositoryTests
	{
		private ScheduledJobRepository CreateScheduledJobRepository()
		{
			return new ScheduledJobRepository(new MongoDBConfiguration() { DatabaseName = "Syringe-Tests" });
		}

		[SetUp]
		public void SetUp()
		{
			CreateScheduledJobRepository().Wipe();
		}

		[Test]
		public async Task AddJob_should_store_a_job()
		{
			// Arrange
			var expectedJob = new ScheduledJob()
			{
				Id = Guid.NewGuid(),
				Crontab = "00 24 * * *",
				Description = "Every day at 12am",
				TeamId = Guid.NewGuid(),
				TestCaseFilename = "smoke-tests.xml"
			};

			var repository = CreateScheduledJobRepository();

			// Act
			await repository.AddJobAsync(expectedJob);

			// Assert
			IEnumerable<ScheduledJob> jobs = repository.GetAll();

			Assert.That(jobs.Count(), Is.EqualTo(1));

			ScheduledJob actualJob = jobs.FirstOrDefault();
			Assert.That(actualJob.Id, Is.EqualTo(expectedJob.Id));
			Assert.That(actualJob.Crontab, Is.EqualTo(expectedJob.Crontab));
			Assert.That(actualJob.Description, Is.EqualTo(expectedJob.Description));
			Assert.That(actualJob.TeamId, Is.EqualTo(expectedJob.TeamId));
			Assert.That(actualJob.TestCaseFilename, Is.EqualTo(expectedJob.TestCaseFilename));
		}
	}
}