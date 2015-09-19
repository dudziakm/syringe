using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Database.Indexing;
using Syringe.Core.TestCases;

namespace Syringe.Core.Results
{
	/// <summary>
	/// http://localhost:32345/studio/index.html#databases/documents?&database=syringe
	/// </summary>
	public class RavenDbTestCaseSessionRepository
	{
		private DocumentStore _documentStore;

		public RavenDbTestCaseSessionRepository()
		{
			_documentStore = new DocumentStore
			{
				DefaultDatabase = "syringe",
				Url = "http://localhost:32345"
			};
			_documentStore.Initialize();
		}

		public void Save(TestCaseSession session)
		{
			using (IDocumentSession documentSession = _documentStore.OpenSession())
			{
				documentSession.Store(session);
				documentSession.SaveChanges();
			}
		}

		public IEnumerable<SessionInfo> LoadAll()
		{
			using (IDocumentSession documentSession = _documentStore.OpenSession())
			{
				var items = documentSession.Query<TestCaseSession>().Select(x => new SessionInfo() {Id = x.Id});
				return items;
			}
		}

		public TestCaseSession GetById(Guid id)
		{
			using (IDocumentSession documentSession = _documentStore.OpenSession())
			{
				return documentSession.Load<TestCaseSession>(id);
			}
		}

		public void Delete(Guid id)
		{
			using (IDocumentSession documentSession = _documentStore.OpenSession())
			{
				documentSession.Delete<TestCaseSession>(id);
				documentSession.SaveChanges();
			}
		}
	}
}