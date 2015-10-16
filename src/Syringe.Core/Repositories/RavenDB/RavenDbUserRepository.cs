using System;
using System.Collections.Generic;
using Raven.Client;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories.RavenDB
{
	public class RavenDbUserRepository : IUserRepository, IDisposable
	{
		private readonly IDocumentSession _documentSession;

		public RavenDbUserRepository(IDocumentStore documentStore)
		{
			_documentSession = documentStore.OpenSession();
		}

		public void AddUser(User user)
		{
			user.Password = User.HashPassword(user.Password);

			_documentSession.Store(user);
			_documentSession.SaveChanges();
		}

		public void UpdateUser(User user, bool passwordHasChanged)
		{
			if (passwordHasChanged)
				user.Password = User.HashPassword(user.Password);

			_documentSession.Store(user);
			_documentSession.SaveChanges();
		}

		public void Wipe()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<User> GetUsers()
		{
			return _documentSession.Query<User>();
		}

		public IEnumerable<User> GetUsersInTeam(Team team)
		{
			throw new NotImplementedException();
		}

		public void DeleteUser(User user)
		{
			_documentSession.Delete<User>(user);
			_documentSession.SaveChanges();
		}

		public void Dispose()
		{
			_documentSession?.Dispose();
		}
	}
}