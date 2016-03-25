using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Syringe.Core.Repositories;
using Syringe.Core.Results;
using Syringe.Core.Services;
using Syringe.Core.TestCases;

namespace Syringe.Service.Api
{
    // TODO: Tests
    public class CasesController : ApiController, ICaseService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly ITestCaseSessionRepository _testCaseSessionRepository;

        public CasesController(ICaseRepository caseRepository, ITestCaseSessionRepository testCaseSessionRepository)
        {
            _caseRepository = caseRepository;
            _testCaseSessionRepository = testCaseSessionRepository;
        }

        [Route("api/cases/ListForTeam")]
        [HttpGet]
        public IEnumerable<string> ListFilesForTeam(string teamName)
        {
            return _caseRepository.ListCasesForTeam(teamName);
        }

        [Route("api/cases/GetTestCase")]
        [HttpGet]
        public Case GetTestCase(string filename, string teamName, Guid caseId)
        {
            return _caseRepository.GetTestCase(filename, teamName, caseId);
        }

        [Route("api/cases/GetTestCaseCollection")]
        [HttpGet]
        public CaseCollection GetTestCaseCollection(string filename, string teamName)
        {
            return _caseRepository.GetTestCaseCollection(filename, teamName);
        }
        [Route("api/cases/GetXmlTestCaseCollection")]
        [HttpGet]
        public string GetXmlTestCaseCollection(string filename, string teamName)
        {
            return _caseRepository.GetXmlTestCaseCollection(filename, teamName);
        }

        [Route("api/cases/EditTestCase")]
        [HttpPost]
        public bool EditTestCase([FromBody]Case testCase, [FromUri]string teamName)
        {
            return _caseRepository.SaveTestCase(testCase, teamName);
        }

        [Route("api/cases/CreateTestCase")]
        [HttpPost]
        public bool CreateTestCase([FromBody]Case testCase, [FromUri]string teamName)
        {
            return _caseRepository.CreateTestCase(testCase, teamName);
        }

        [Route("api/cases/DeleteTestCase")]
        [HttpPost]
        public bool DeleteTestCase(Guid testCaseId, string fileName, string teamName)
        {
            return _caseRepository.DeleteTestCase(testCaseId, fileName, teamName);
        }

        [Route("api/cases/CreateTestFile")]
        [HttpPost]
        public bool CreateTestFile([FromBody]CaseCollection caseCollection, [FromUri]string teamName)
        {
            return _caseRepository.CreateTestFile(caseCollection, teamName);
        }

        [Route("api/cases/UpdateTestFile")]
        [HttpPost]
        public bool UpdateTestFile([FromBody]CaseCollection caseCollection, [FromUri]string teamName)
        {
            return _caseRepository.UpdateTestFile(caseCollection, teamName);
        }

        [Route("api/cases/GetSummariesForToday")]
        [HttpGet]
        public IEnumerable<SessionInfo> GetSummariesForToday()
        {
            return _testCaseSessionRepository.GetSummariesForToday();
        }

        [Route("api/cases/GetSummaries")]
        [HttpGet]
        public IEnumerable<SessionInfo> GetSummaries()
        {
            return _testCaseSessionRepository.GetSummaries();
        }

        [Route("api/cases/GetById")]
        [HttpGet]
        public TestCaseSession GetById(Guid caseId)
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