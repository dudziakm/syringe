using System.Collections.Generic;

namespace Syringe.Core.Domain.Services
{
	public interface ICaseService
	{
		IEnumerable<string> ListFilesForTeam(string teamName);
		Case GetTestCase(string filename, string teamName, int caseId);
		CaseCollection GetTestCaseCollection(string filename, string teamName);
		bool AddTestCase(Case testCase, string teamName);
	}
}