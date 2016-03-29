using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Core.Services
{
	public interface ICaseService
	{
		IEnumerable<string> ListFilesForTeam(string teamName);
		Test GetTestCase(string filename, string teamName, Guid caseId);
		TestFile GetTestCaseCollection(string filename, string teamName);
	    string GetXmlTestCaseCollection(string filename, string teamName);
        bool EditTestCase(Test testTest, string teamName);
	    bool CreateTestCase(Test testTest, string teamName);
        bool DeleteTestCase(Guid testCaseId, string fileName, string teamName);
	    bool CreateTestFile(TestFile testFile, string teamName);
	    bool UpdateTestFile(TestFile testFile, string teamName);
	    IEnumerable<TestFileResultSummary> GetSummariesForToday();
	    IEnumerable<TestFileResultSummary> GetSummaries();
        TestFileResult GetById(Guid caseId);
        Task DeleteAsync(Guid sessionId);
	}
}