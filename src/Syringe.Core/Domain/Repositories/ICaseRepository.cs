using System.Collections.Generic;

namespace Syringe.Core.Domain.Repositories
{
    public interface ICaseRepository
    {
	    IEnumerable<string> ListCasesForTeam(string teamName);
		CaseCollection GetTestCaseCollection(string filename, string teamName);
        Case GetTestCase(string filename, string teamName, int caseId);
		bool SaveTestCase(Case testCase, string teamName);
    }
}