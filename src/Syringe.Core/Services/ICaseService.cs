using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Results;
using Syringe.Core.TestCases;

namespace Syringe.Core.Services
{
	public interface ICaseService
	{
		IEnumerable<string> ListFilesForTeam(string teamName);
		Case GetTestCase(string filename, string teamName, Guid caseId);
		CaseCollection GetTestCaseCollection(string filename, string teamName);
        bool EditTestCase(Case testCase, string teamName);
	    bool CreateTestCase(Case testCase, string teamName);
        bool DeleteTestCase(Guid testCaseId, string fileName, string teamName);
	    bool CreateTestFile(CaseCollection caseCollection, string teamName);
	    bool UpdateTestFile(CaseCollection caseCollection, string teamName);
	    IEnumerable<SessionInfo> GetSummariesForToday();
	    IEnumerable<SessionInfo> GetSummaries();
        TestCaseSession GetById(Guid caseId);
        Task DeleteAsync(Guid sessionId);
	}
}