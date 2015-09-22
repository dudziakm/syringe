using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Document;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories.RavenDB
{
	public class RavenDbTeamRepository : ITeamRepository, IDisposable
	{
		private readonly IDocumentSession _documentSession;

		public RavenDbTeamRepository(IDocumentStore documentStore)
		{
			_documentSession = documentStore.OpenSession();
		}

		public void AddTeam(Team team)
		{
			_documentSession.Store(team);
			_documentSession.SaveChanges();
		}

		public void UpdateTeam(Team team)
		{
			_documentSession.Store(team);
			_documentSession.SaveChanges();
		}

		public void Delete(Team team)
		{
			_documentSession.Delete<Team>(team);
			_documentSession.SaveChanges();
		}

		public IEnumerable<User> GetUsersInTeam(Team team)
		{
			if (team == null)
				throw new ArgumentNullException("team");

			if (team.UserIds.Count == 0)
				return new List<User>();

			IEnumerable<string> ids = team.UserIds.Select(id => "users/"+id);

			return _documentSession.Load<User>(ids);
		}

		public void AddUserToTeam(Team team, User user)
		{
			if (team == null)
				throw new ArgumentNullException("team");

			if (user == null)
				throw new ArgumentNullException("user");

			team.UserIds.Add(user.Id);
			_documentSession.Store(team);
			_documentSession.SaveChanges();
		}

		public void RemoveUserFromTeam(Team team, User user)
		{
			if (team == null)
				throw new ArgumentNullException("team");

			if (user == null)
				throw new ArgumentNullException("user");

			team.UserIds.Remove(user.Id);
			_documentSession.Store(team);
			_documentSession.SaveChanges();
		}

		public IEnumerable<Team> GetTeams()
		{
			return _documentSession.Query<Team>();
		}

		public Team GetTeam(string name)
		{
			return _documentSession.Query<Team>().FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		}

		public void Dispose()
		{
			_documentSession?.Dispose();
		}
	}
}