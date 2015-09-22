using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Document;
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

		public void Save(TestCaseSession session)
		{
			_documentSession.Store(session);
			_documentSession.SaveChanges();
		}

		public IEnumerable<SessionInfo> LoadAll()
		{
			var items = _documentSession.Query<TestCaseSession>().Select(x => new SessionInfo() { Id = x.Id });
			return items;
		}

		public TestCaseSession GetById(Guid id)
		{
			return _documentSession.Load<TestCaseSession>(id);
		}

		public void Delete(Guid id)
		{
			_documentSession.Delete<TestCaseSession>(id);
			_documentSession.SaveChanges();
		}

		public IEnumerable<SessionInfo> ResultsForToday()
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