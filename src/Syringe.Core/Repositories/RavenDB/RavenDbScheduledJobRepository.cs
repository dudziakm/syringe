using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Document;
using Syringe.Core.Schedule;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories.RavenDB
{
	public class RavenDbScheduledJobRepository : IScheduledJobRepository, IDisposable
	{
		private readonly IDocumentSession _documentSession;

		public RavenDbScheduledJobRepository(IDocumentStore documentStore)
		{
			_documentSession = documentStore.OpenSession();	
        }

		public void AddJob(ScheduledJob job)
		{
			_documentSession.Store(job);
			_documentSession.SaveChanges();
		}

		public void UpdateJob(ScheduledJob job)
		{
			_documentSession.Store(job);
			_documentSession.SaveChanges();
		}

		public void DeleteJob(ScheduledJob job)
		{
			_documentSession.Delete<ScheduledJob>(job);
			_documentSession.SaveChanges();
		}

		public IEnumerable<ScheduledJob> GetAll()
		{
			return _documentSession.Query<ScheduledJob>();
		}

		public IEnumerable<ScheduledJob> GetForTeam(Team team)
		{
			return _documentSession.Query<ScheduledJob>().Where(x => x.TeamId == team.Id);
		}

		public void Dispose()
		{
			_documentSession?.Dispose();
		}
	}
}