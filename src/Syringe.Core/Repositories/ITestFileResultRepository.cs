using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Tests.Results;

namespace Syringe.Core.Repositories
{
	public interface ITestFileResultRepository
	{
		Task AddAsync(TestFileResult testFileResult);
		Task DeleteAsync(Guid testFileResultId);
		TestFileResult GetById(Guid id);
		void Wipe();
		IEnumerable<TestFileResultSummary> GetSummaries();
		IEnumerable<TestFileResultSummary> GetSummariesForToday();
	}
}