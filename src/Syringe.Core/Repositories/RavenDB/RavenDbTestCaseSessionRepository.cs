using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Syringe.Core.Results;

namespace Syringe.Core.Repositories.RavenDB
{
	public class RavenDbTestCaseSessionRepository : ITestCaseSessionRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenDbTestCaseSessionRepository(IDocumentStore documentStore)
		{
			_documentSession = documentStore.OpenSession();
		}

		public void Add(TestCaseSession session)
		{
			_documentSession.Store(session);
			_documentSession.SaveChanges();
		}

		public void Wipe()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SessionInfo> GetSummaries()
		{
			var items = _documentSession.Query<TestCaseSession>().Select(x => new SessionInfo() { Id = x.Id });
			return items;
		}

		public TestCaseSession GetById(Guid id)
		{
			return _documentSession.Load<TestCaseSession>(id);
		}

		public void Delete(TestCaseSession session)
		{
			_documentSession.Delete<TestCaseSession>(session);
			_documentSession.SaveChanges();
		}

		public IEnumerable<SessionInfo> GetSummariesForToday()
		{
			DateTime today = DateTime.UtcNow.Date;

			var items = _documentSession.Query<TestCaseSession>()
				.Where(x => x.StartTime.Date == today)
				.Select(x => new SessionInfo() { Id = x.Id });
			return items;
		}

		public void Dispose()
		{
			_documentSession?.Dispose();
		}
	}
}