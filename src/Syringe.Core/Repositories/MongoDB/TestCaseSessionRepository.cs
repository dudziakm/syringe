﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Syringe.Core.Tests.Results;

namespace Syringe.Core.Repositories.MongoDB
{
    public class TestCaseSessionRepository : ITestCaseSessionRepository
    {
        private static readonly string COLLECTION_NAME = "TestCaseSessions";
        private readonly MongoDBConfiguration _mongoDbConfiguration;
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TestFileResult> _collection;

        public TestCaseSessionRepository(MongoDBConfiguration mongoDbConfiguration)
        {
            _mongoDbConfiguration = mongoDbConfiguration;
            _mongoClient = new MongoClient(_mongoDbConfiguration.ConnectionString);
            _database = _mongoClient.GetDatabase(_mongoDbConfiguration.DatabaseName);
            _collection = _database.GetCollection<TestFileResult>(COLLECTION_NAME);
        }

        public async Task AddAsync(TestFileResult session)
        {
            await _collection.InsertOneAsync(session);
        }

        public async Task DeleteAsync(Guid sessionId)
        {
            await _collection.DeleteOneAsync(x => x.Id == sessionId);
        }

        public TestFileResult GetById(Guid id)
        {
            return _collection.AsQueryable().ToList().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<TestFileResultSummary> GetSummaries()
        {
            return _collection.AsQueryable()
                                .ToList()
                                .Select(x => new TestFileResultSummary()
                                {
                                    Id = x.Id,
                                    DateRun = x.StartTime,
                                    TestCaseFileName = x.Filename,
                                    TotalRunTime = x.TotalRunTime,
                                    TotalPassed = x.TotalTestsPassed,
                                    TotalFailed = x.TotalTestsFailed,
                                    TotalRun = x.TotalTestsRun
                                })
                                .OrderByDescending(x => x.DateRun);
        }

        public IEnumerable<TestFileResultSummary> GetSummariesForToday()
        {
            return _collection.AsQueryable()
                                .Where(x => x.StartTime >= DateTime.Today)
                                 .ToList()
                                .Select(x => new TestFileResultSummary()
                                {
                                    Id = x.Id,
                                    DateRun = x.StartTime,
                                    TestCaseFileName = x.Filename,
                                    TotalRunTime = x.TotalRunTime,
                                    TotalPassed = x.TotalTestsPassed,
                                    TotalFailed = x.TotalTestsFailed,
                                    TotalRun = x.TotalTestsRun
                                })
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
