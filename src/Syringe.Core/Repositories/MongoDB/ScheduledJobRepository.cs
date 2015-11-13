using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Syringe.Core.Schedule;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories.MongoDB
{
	public class ScheduledJobRepository : IScheduledJobRepository
	{
		private static readonly string COLLECTION_NAME = "ScheduledJobs";
		private readonly Configuration _configuration;
		private readonly MongoClient _mongoClient;
		private readonly IMongoDatabase _database;
		private readonly IMongoCollection<ScheduledJob> _collection;

		public ScheduledJobRepository(Configuration configuration)
		{
			_configuration = configuration;
			_mongoClient = new MongoClient(_configuration.ConnectionString);
			_database = _mongoClient.GetDatabase(_configuration.DatabaseName);
			_collection = _database.GetCollection<ScheduledJob>(COLLECTION_NAME);
		}

		public void AddJob(ScheduledJob job)
		{
			_collection.InsertOneAsync(job).Wait();
		}

		public void UpdateJob(ScheduledJob job)
		{
			_collection.ReplaceOneAsync(j => j.Id == job.Id, job).Wait();
		}

		public void DeleteJob(ScheduledJob job)
		{
			_collection.DeleteOneAsync(j => j.Id == job.Id).Wait();
		}

		public IEnumerable<ScheduledJob> GetAll()
		{
			return _collection.AsQueryable().ToList();
		}

		public IEnumerable<ScheduledJob> GetForTeam(Team team)
		{
			return _collection.AsQueryable().Where(j => j.TeamId == team.Id).ToList();
		}

		public void Wipe()
		{
			_database.DropCollectionAsync(COLLECTION_NAME).Wait();
		}
	}
}
