using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Syringe.Core.Repositories;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Service.Api
{
    public class TestsController : ApiController, ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly ITestCaseSessionRepository _testCaseSessionRepository;

        public TestsController(ITestRepository testRepository, ITestCaseSessionRepository testCaseSessionRepository)
        {
            _testRepository = testRepository;
            _testCaseSessionRepository = testCaseSessionRepository;
        }

        [Route("api/tests/ListForTeam")]
        [HttpGet]
        public IEnumerable<string> ListFilesForTeam(string branchName)
        {
            return _testRepository.ListFilesForBranch(branchName);
        }

        [Route("api/tests/GetTest")]
        [HttpGet]
        public Test GetTest(string filename, string branchName, Guid testId)
        {
            return _testRepository.GetTest(filename, branchName, testId);
        }

        [Route("api/tests/GetTestFile")]
        [HttpGet]
        public TestFile GetTestFile(string filename, string branchName)
        {
            return _testRepository.GetTestFile(filename, branchName);
        }
        [Route("api/tests/GetXml")]
        [HttpGet]
        public string GetXml(string filename, string branchName)
        {
            return _testRepository.GetXml(filename, branchName);
        }

        [Route("api/tests/EditTestCase")]
        [HttpPost]
        public bool EditTest([FromBody]Test test, [FromUri]string branchName)
        {
            return _testRepository.SaveTest(test, branchName);
        }

        [Route("api/tests/CreateTest")]
        [HttpPost]
        public bool CreateTest([FromBody]Test testTest, [FromUri]string branchName)
        {
            return _testRepository.CreateTest(testTest, branchName);
        }

        [Route("api/tests/DeleteTest")]
        [HttpPost]
        public bool DeleteTest(Guid testId, string fileName, string branchName)
        {
            return _testRepository.DeleteTest(testId, fileName, branchName);
        }

        [Route("api/tests/CreateTestFile")]
        [HttpPost]
        public bool CreateTestFile([FromBody]TestFile testFile, [FromUri]string branchName)
        {
            return _testRepository.CreateTestFile(testFile, branchName);
        }

        [Route("api/tests/UpdateTestFile")]
        [HttpPost]
        public bool UpdateTestFile([FromBody]TestFile testFile, [FromUri]string branchName)
        {
            return _testRepository.UpdateTestFile(testFile, branchName);
        }

        [Route("api/tests/GetSummariesForToday")]
        [HttpGet]
        public IEnumerable<TestFileResultSummary> GetSummariesForToday()
        {
            return _testCaseSessionRepository.GetSummariesForToday();
        }

        [Route("api/tests/GetSummaries")]
        [HttpGet]
        public IEnumerable<TestFileResultSummary> GetSummaries()
        {
            return _testCaseSessionRepository.GetSummaries();
        }

        [Route("api/tests/GetById")]
        [HttpGet]
        public TestFileResult GetResultById(Guid id)
        {
            return _testCaseSessionRepository.GetById(id);
        }

        [Route("api/tests/DeleteAsync")]
        [HttpPost]
        public Task DeleteAsync(Guid sessionId)
        {
            return _testCaseSessionRepository.DeleteAsync(sessionId);
        }
    }
}