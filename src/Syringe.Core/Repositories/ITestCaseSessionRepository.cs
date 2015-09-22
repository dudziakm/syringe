using System;
using System.Collections.Generic;
using Syringe.Core.Results;

namespace Syringe.Core.Repositories
{
	public interface ITestCaseSessionRepository : IDisposable
	{
		void Delete(Guid id);
		TestCaseSession GetById(Guid id);
		IEnumerable<SessionInfo> LoadAll();
		IEnumerable<SessionInfo> ResultsForToday();
		void Save(TestCaseSession session);
	}
}