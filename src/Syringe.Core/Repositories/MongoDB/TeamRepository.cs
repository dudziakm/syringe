using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories.MongoDB
{
	public class TeamRepository : ITeamRepository
	{
		private static readonly string COLLECTION_NAME = "Teams";

		private readonly Configuration _configuration;
		private readonly MongoClient _mongoClient;
		private readonly IMongoDatabase _database;
		private readonly IMongoCollection<Team> _collection;

		public TeamRepository(Configuration configuration)
		{
			_configuration = configuration;
			_mongoClient = new MongoClient(_configuration.ConnectionString);
			_database = _mongoClient.GetDatabase(_configuration.DatabaseName);
			_collection = _database.GetCollection<Team>(COLLECTION_NAME);
		}

		/// <summary>
		/// Adds a team to the database.
		/// </summary>
		public void AddTeam(Team team)
		{
			_collection.InsertOneAsync(team).Wait();
		}

		/// <summary>
		/// Updates (replaces) a team in the database, based on their id.
		/// </summary>
		public void UpdateTeam(Team team)
		{
			_collection.ReplaceOneAsync(t => t.Id == team.Id, team).Wait();
		}

		/// <summary>
		/// Removes a team from the database, based on their id.
		/// </summary>
		public void Delete(Team team)
		{
			_collection.DeleteOneAsync(t => t.Id == team.Id).Wait();
		}

		/// <summary>
		/// Adds the user's id to the team's list of users.
		/// </summary>
		public void AddUserToTeam(Team team, User user)
		{
			team.UserIds.Add(user.Id);
			UpdateTeam(team);
		}

		/// <summary>
		/// Removes the user's id to the team's list of users.
		/// </summary>
		public void RemoveUserFromTeam(Team team, User user)
		{
			team.UserIds.Remove(user.Id);
			UpdateTeam(team);
		}

		/// <summary>
		/// Gets all teams in the database.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Team> GetTeams()
		{
			return _collection.AsQueryable().ToList();
		}

		/// <summary>
		/// Gets the first team that matches the given name. The search isn't case sensitive.
		/// </summary>
		public Team GetTeam(string name)
		{
			name = name.ToLower();
            return _collection.AsQueryable().FirstOrDefault(t => t.Name.ToLower().Contains(name));
		}

		/// <summary>
		/// Removes all teams from the database.
		/// </summary>
		public void Wipe()
		{
			_database.DropCollectionAsync(COLLECTION_NAME).Wait();
		}
	}
}
