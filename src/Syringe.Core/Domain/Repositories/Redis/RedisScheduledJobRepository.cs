using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using Syringe.Core.Domain.Entities;

namespace Syringe.Core.Domain.Repositories.Redis
{
	public class RedisScheduledJobRepository : IScheduledJobRepository, IDisposable
	{
		internal RedisClient RedisClient { get; set; }
		internal RedisTypedClient<ScheduledJob> RedisScheduledJobClient { get; set; }

		public RedisScheduledJobRepository() : this("localhost", 6379)
		{
		}

		public RedisScheduledJobRepository(string host, int port)
		{
			if (host == null) 
				throw new ArgumentNullException("host");

			if (port < 1)
				throw new ArgumentException("port is is invalid", "port");

			RedisClient = new RedisClient(host, port);
			RedisScheduledJobClient = new RedisTypedClient<ScheduledJob>(RedisClient);
		}

		public void AddJob(ScheduledJob job)
		{
			RedisScheduledJobClient.Store(job);
		}

		public void UpdateJob(ScheduledJob job)
		{
			RedisScheduledJobClient.Store(job);
		}

		public void DeleteJob(ScheduledJob job)
		{
			RedisScheduledJobClient.Delete(job);
		}

		public IEnumerable<ScheduledJob> GetAll()
		{
			return RedisScheduledJobClient.GetAll();
		}

		public IEnumerable<ScheduledJob> GetForTeam(Team team)
		{
			return GetAll().Where(x => x.TeamId == team.Id);
		}

		public void Dispose()
		{
			if (RedisClient != null)
				RedisClient.Dispose();
		}
	}
}