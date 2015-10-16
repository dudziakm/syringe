using System;
using System.Collections.Generic;
using Syringe.Core.Results;

namespace Syringe.Core.Repositories
{
	public interface ITestCaseSessionRepository
	{
		void Add(TestCaseSession session);
		void Delete(TestCaseSession session);
		TestCaseSession GetById(Guid id);
		void Wipe();

		IEnumerable<SessionInfo> GetSummaries();
		IEnumerable<SessionInfo> GetSummariesForToday();
	}
}