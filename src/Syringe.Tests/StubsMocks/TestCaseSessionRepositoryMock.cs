using System;
using System.Collections.Generic;
using Syringe.Core.Repositories;
using Syringe.Core.Results;

namespace Syringe.Tests.StubsMocks
{
	internal class TestCaseSessionRepositoryMock : ITestCaseSessionRepository
	{
		public TestCaseSession SavedSession { get; set; }

		public void Delete(TestCaseSession session)
		{
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

		public void Add(TestCaseSession session)
		{
			SavedSession = session;
		}
	}
}