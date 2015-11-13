using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Syringe.Core.Results;

namespace Syringe.Core.Repositories.MongoDB
{
    public class TestCaseSessionRepository : ITestCaseSessionRepository
    {
        private static readonly string COLLECTION_NAME = "TestCaseSessions";
        private readonly Configuration _configuration;
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TestCaseSession> _collection;

        public TestCaseSessionRepository(Configuration configuration)
        {
            _configuration = configuration;
            _mongoClient = new MongoClient(_configuration.ConnectionString);
            _database = _mongoClient.GetDatabase(_configuration.DatabaseName);
            _collection = _database.GetCollection<TestCaseSession>(COLLECTION_NAME);
        }

        public async Task AddAsync(TestCaseSession session)
        {
            await _collection.InsertOneAsync(session);
        }

        public async Task DeleteAsync(TestCaseSession session)
        {
            await _collection.DeleteOneAsync(x => x.Id == session.Id);
        }

        public TestCaseSession GetById(Guid id)
        {
            return _collection.AsQueryable().ToList().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<SessionInfo> GetSummaries()
        {
            return _collection.AsQueryable()
                                .ToList()
                                .Select(x => new SessionInfo() { Id = x.Id, DateRun = x.StartTime, TestCaseFileName = x.TestCaseFilename})
                                .OrderByDescending(x => x.DateRun);
        }

        public IEnumerable<SessionInfo> GetSummariesForToday()
        {
            return _collection.AsQueryable()
                                .Where(x => x.StartTime >= DateTime.Today)
                                .ToList()
                                .Select(x => new SessionInfo() { Id = x.Id, DateRun = x.StartTime, TestCaseFileName = x.TestCaseFilename })
                                .OrderByDescending(x => x.DateRun);
        }

        /// <summary>
        /// Removes all TestCaseSession objects from the database.
        /// </summary>
        public void Wipe()
        {
            _database.DropCollectionAsync(COLLECTION_NAME).Wait();
        }
    }
}
