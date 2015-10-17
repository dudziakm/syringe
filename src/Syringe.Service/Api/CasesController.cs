using System.Collections.Generic;
using System.Web.Http;
using Syringe.Core.Repositories;
using Syringe.Core.Services;
using Syringe.Core.TestCases;

namespace Syringe.Service.Api
{
    // TODO: Tests
    public class CasesController : ApiController, ICaseService
    {
        private readonly ICaseRepository _caseRepository;

        public CasesController() : this(new CaseRepository()) { }

        internal CasesController(ICaseRepository caseRepository)
        {
            _caseRepository = caseRepository;
        }

        [Route("api/cases/ListForTeam")]
        [HttpGet]
        public IEnumerable<string> ListFilesForTeam(string teamName)
        {
            return _caseRepository.ListCasesForTeam(teamName);
        }

        [Route("api/cases/GetTestCase")]
        [HttpGet]
        public Case GetTestCase(string filename, string teamName, int caseId)
        {
            return _caseRepository.GetTestCase(filename, teamName, caseId);
        }

        [Route("api/cases/GetTestCaseCollection")]
        [HttpGet]
        public CaseCollection GetTestCaseCollection(string filename, string teamName)
        {
            return _caseRepository.GetTestCaseCollection(filename, teamName);
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
        public bool DeleteTestCase(int testCaseId, string fileName, string teamName)
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
    }
}