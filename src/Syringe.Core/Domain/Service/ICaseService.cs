using System.Collections.Generic;

namespace Syringe.Core.Domain.Service
{
	public interface ICaseService
	{
		IEnumerable<string> ListFilesForTeam(string teamName);
		Case GetTestCase(string filename, string teamName, int caseId);
		CaseCollection GetTestCaseCollection(string filename, string teamName);
	}
}