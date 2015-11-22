using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Results;

namespace Syringe.Core.Repositories
{
	public interface ITestCaseSessionRepository
	{
		Task AddAsync(TestCaseSession session);
		Task DeleteAsync(Guid sessionId);
		TestCaseSession GetById(Guid id);
		void Wipe();
		IEnumerable<SessionInfo> GetSummaries();
		IEnumerable<SessionInfo> GetSummariesForToday();
	}
}