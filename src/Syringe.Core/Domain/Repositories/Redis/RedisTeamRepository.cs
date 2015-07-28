using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using Syringe.Core.Domain.Entities;

namespace Syringe.Core.Domain.Repositories.Redis
{
	public class RedisTeamRepository : ITeamRepository, IDisposable
	{
		internal RedisClient RedisClient { get; set; }
		internal RedisTypedClient<Team> RedisTeamClient { get; set; }
		internal RedisTypedClient<User> RedisUserClient { get; set; }

		public RedisTeamRepository() : this("localhost", 6379)
		{
		}

		public RedisTeamRepository(string host, int port)
		{
			if (host == null) 
				throw new ArgumentNullException("host");

			if (port < 1)
				throw new ArgumentException("port is is invalid", "port");

			RedisClient = new RedisClient(host, port);
			RedisTeamClient = new RedisTypedClient<Team>(RedisClient);
			RedisUserClient = new RedisTypedClient<User>(RedisClient);
		}

		public void AddTeam(Team team)
		{
			RedisTeamClient.Store(team);
		}

		public void UpdateTeam(Team team)
		{
			RedisTeamClient.Store(team);
		}

		public void Delete(Team team)
		{
			RedisTeamClient.Delete(team);
		}

		public IEnumerable<User> GetUsersInTeam(Team team)
		{
			if (team == null) 
				throw new ArgumentNullException("team");

			return RedisUserClient.GetByIds(team.UserIds);
		}

		public void AddUserToTeam(Team team, User user)
		{
			if (team == null) 
				throw new ArgumentNullException("team");

			if (user == null) 
				throw new ArgumentNullException("user");

			team.UserIds.Add(user.Id);
			RedisTeamClient.Store(team);
		}

		public void RemoveUserFromTeam(Team team, User user)
		{
			if (team == null)
				throw new ArgumentNullException("team");

			if (user == null)
				throw new ArgumentNullException("user");

			team.UserIds.Remove(user.Id);
			RedisTeamClient.Store(team);
		}

		public IEnumerable<Team> GetTeams()
		{
			return RedisTeamClient.GetAll();
		}

		public Team GetTeam(string name)
		{
			return GetTeams().FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		}

		public void Dispose()
		{
			if (RedisClient != null)
				RedisClient.Dispose();
		}
	}
}