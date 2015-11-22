using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Repositories;
using Syringe.Core.Results;

namespace Syringe.Tests.StubsMocks
{
	internal class TestCaseSessionRepositoryMock : ITestCaseSessionRepository
	{
		public TestCaseSession SavedSession { get; set; }

		public Task DeleteAsync(Guid session)
		{
			return Task.FromResult<object>(null);
		}

		public TestCaseSession GetById(Guid id)
		{
			return new TestCaseSession();
		}

		public void Wipe()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SessionInfo> GetSummaries()
		{
			return new List<SessionInfo>();
		}

		public IEnumerable<SessionInfo> GetSummariesForToday()
		{
			return new List<SessionInfo>();
		}

		public Task AddAsync(TestCaseSession session)
		{
			SavedSession = session;
		    return Task.FromResult<object>(null);
		}
	}
}