using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Syringe.Core.Schedule;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories.MongoDB
{
	public class ScheduledJobRepository : IScheduledJobRepository
	{
		private static readonly string COLLECTION_NAME = "ScheduledJobs";
		private readonly MongoDBConfiguration _mongoDbConfiguration;
		private readonly MongoClient _mongoClient;
		private readonly IMongoDatabase _database;
		private readonly IMongoCollection<ScheduledJob> _collection;

		public ScheduledJobRepository(MongoDBConfiguration mongoDbConfiguration)
		{
			_mongoDbConfiguration = mongoDbConfiguration;
			_mongoClient = new MongoClient(_mongoDbConfiguration.ConnectionString);
			_database = _mongoClient.GetDatabase(_mongoDbConfiguration.DatabaseName);
			_collection = _database.GetCollection<ScheduledJob>(COLLECTION_NAME);
		}

		public async Task AddJobAsync(ScheduledJob job)
		{
			await _collection.InsertOneAsync(job);
		}

		public async Task UpdateJobAsync(ScheduledJob job)
		{
			await _collection.ReplaceOneAsync(j => j.Id == job.Id, job);
		}

		public async Task DeleteJobAsync(ScheduledJob job)
		{
		    await _collection.DeleteOneAsync(j => j.Id == job.Id);
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
