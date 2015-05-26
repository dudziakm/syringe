using System;
using System.Collections.Generic;
using Quartz;
using Quartz.Impl;
using Syringe.Core.Domain.Entities;

namespace Syringe.Service.Schedule
{
	public class Scheduler
	{
		public static void Start()
		{
			IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
			scheduler.Start();
		}

		public static void Stop()
		{
			IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
			scheduler.Shutdown();
		}

		public void ReScheduleAllJobs()
		{
			
		}

		public void ScheduleJob(string jobName, string testCaseFilename, Team team, string cronTab)
		{
			IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

			IJobDetail job = JobBuilder.Create<QuartzJob>()
					.WithIdentity(jobName, team.Name)
					.UsingJobData("TeamName", team.Name)
					.UsingJobData("TeamId", team.Id.ToString())
					.UsingJobData("TestCaseFilename", testCaseFilename)
					.Build();

			ITrigger trigger = TriggerBuilder.Create()
					.WithIdentity("trigger1", "group1")
					.WithCronSchedule(cronTab)
					.Build();

			scheduler.ScheduleJob(job, trigger);
		}
	}
}
