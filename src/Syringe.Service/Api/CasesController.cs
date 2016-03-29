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
    public class CasesController : ApiController, ICaseService
    {
        private readonly ITestRepository _testRepository;
        private readonly ITestCaseSessionRepository _testCaseSessionRepository;

        public CasesController(ITestRepository testRepository, ITestCaseSessionRepository testCaseSessionRepository)
        {
            _testRepository = testRepository;
            _testCaseSessionRepository = testCaseSessionRepository;
        }

        [Route("api/cases/ListForTeam")]
        [HttpGet]
        public IEnumerable<string> ListFilesForTeam(string teamName)
        {
            return _testRepository.ListFilesForBranch(teamName);
        }

        [Route("api/cases/GetTest")]
        [HttpGet]
        public Test GetTestCase(string filename, string teamName, Guid caseId)
        {
            return _testRepository.GetTest(filename, teamName, caseId);
        }

        [Route("api/cases/GetTestFile")]
        [HttpGet]
        public TestFile GetTestCaseCollection(string filename, string teamName)
        {
            return _testRepository.GetTestFile(filename, teamName);
        }
        [Route("api/cases/GetXml")]
        [HttpGet]
        public string GetXmlTestCaseCollection(string filename, string teamName)
        {
            return _testRepository.GetXml(filename, teamName);
        }

        [Route("api/cases/EditTestCase")]
        [HttpPost]
        public bool EditTestCase([FromBody]Test testTest, [FromUri]string teamName)
        {
            return _testRepository.SaveTest(testTest, teamName);
        }

        [Route("api/cases/CreateTest")]
        [HttpPost]
        public bool CreateTestCase([FromBody]Test testTest, [FromUri]string teamName)
        {
            return _testRepository.CreateTest(testTest, teamName);
        }

        [Route("api/cases/DeleteTest")]
        [HttpPost]
        public bool DeleteTestCase(Guid testCaseId, string fileName, string teamName)
        {
            return _testRepository.DeleteTest(testCaseId, fileName, teamName);
        }

        [Route("api/cases/CreateTestFile")]
        [HttpPost]
        public bool CreateTestFile([FromBody]TestFile testFile, [FromUri]string teamName)
        {
            return _testRepository.CreateTestFile(testFile, teamName);
        }

        [Route("api/cases/UpdateTestFile")]
        [HttpPost]
        public bool UpdateTestFile([FromBody]TestFile testFile, [FromUri]string teamName)
        {
            return _testRepository.UpdateTestFile(testFile, teamName);
        }

        [Route("api/cases/GetSummariesForToday")]
        [HttpGet]
        public IEnumerable<TestFileResultSummary> GetSummariesForToday()
        {
            return _testCaseSessionRepository.GetSummariesForToday();
        }

        [Route("api/cases/GetSummaries")]
        [HttpGet]
        public IEnumerable<TestFileResultSummary> GetSummaries()
        {
            return _testCaseSessionRepository.GetSummaries();
        }

        [Route("api/cases/GetById")]
        [HttpGet]
        public TestFileResult GetById(Guid caseId)
        {
            return _testCaseSessionRepository.GetById(caseId);
        }

        [Route("api/cases/DeleteAsync")]
        [HttpPost]
        public Task DeleteAsync(Guid sessionId)
        {
            return _testCaseSessionRepository.DeleteAsync(sessionId);
        }
    }
}