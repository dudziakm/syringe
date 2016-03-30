using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Repositories;
using Syringe.Core.Tests.Results;

namespace Syringe.Tests.StubsMocks
{
	internal class TestCaseSessionRepositoryMock : ITestCaseSessionRepository
	{
		public TestFileResult SavedSession { get; set; }

		public Task DeleteAsync(Guid session)
		{
			return Task.FromResult<object>(null);
		}

		public TestFileResult GetById(Guid id)
		{
			return new TestFileResult();
		}

		public void Wipe()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TestFileResultSummary> GetSummaries()
		{
			return new List<TestFileResultSummary>();
		}

		public IEnumerable<TestFileResultSummary> GetSummariesForToday()
		{
			return new List<TestFileResultSummary>();
		}

		public Task AddAsync(TestFileResult session)
		{
			SavedSession = session;
		    return Task.FromResult<object>(null);
		}
	}
}