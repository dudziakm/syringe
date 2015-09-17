using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Syringe.Core.Repositories.Redis;
using Syringe.Core.Schedule;

namespace Syringe.Tests.Integration.Repository
{
	public class RedisScheduledJobRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			var scheduledJobsRepository = CreateScheduledJobRepository();
			scheduledJobsRepository.RedisScheduledJobClient.DeleteAll();
		}

		private RedisScheduledJobRepository CreateScheduledJobRepository()
		{
			return new RedisScheduledJobRepository();	
		}

		[Test]
		public void AddJob_should_store_a_job()
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
			repository.AddJob(expectedJob);

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