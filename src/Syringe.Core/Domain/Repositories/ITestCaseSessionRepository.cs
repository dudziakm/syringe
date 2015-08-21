using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syringe.Core.Results;

namespace Syringe.Core.Domain.Repositories
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
