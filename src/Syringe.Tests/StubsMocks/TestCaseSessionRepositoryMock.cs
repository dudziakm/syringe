using System;
using System.Collections.Generic;
using Syringe.Core.Repositories;
using Syringe.Core.Results;

namespace Syringe.Tests.StubsMocks
{
	internal class TestCaseSessionRepositoryMock : ITestCaseSessionRepository
	{
		public TestCaseSession SavedSession { get; set; }

		public void Dispose()
		{
		}

		public void Delete(Guid id)
		{
		}

		public TestCaseSession GetById(Guid id)
		{
			return new TestCaseSession();
		}

		public IEnumerable<SessionInfo> LoadAll()
		{
			return new List<SessionInfo>();
		}

		public IEnumerable<SessionInfo> ResultsForToday()
		{
			return new List<SessionInfo>();
		}

		public void Save(TestCaseSession session)
		{
			SavedSession = session;
		}
	}
}