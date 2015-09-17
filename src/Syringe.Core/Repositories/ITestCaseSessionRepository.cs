using System;
using System.Collections.Generic;
using Syringe.Core.Results;

namespace Syringe.Core.Repositories
{
	public interface ITestCaseSessionRepository
	{
		IEnumerable<Guid> GetAllSesssions();
		IEnumerable<TestCaseSession> GetSessionsForUser();
		IEnumerable<TestCaseSession> GetSessionsForTeam();
		void SavePersonalRun(TestCaseSession session, string username);
		void SaveTeamRun(TestCaseSession session, string teamName);
	}
}
