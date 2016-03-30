using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Tests.Results;

namespace Syringe.Core.Repositories
{
	public interface ITestCaseSessionRepository
	{
		Task AddAsync(TestFileResult session);
		Task DeleteAsync(Guid sessionId);
		TestFileResult GetById(Guid id);
		void Wipe();
		IEnumerable<TestFileResultSummary> GetSummaries();
		IEnumerable<TestFileResultSummary> GetSummariesForToday();
	}
}