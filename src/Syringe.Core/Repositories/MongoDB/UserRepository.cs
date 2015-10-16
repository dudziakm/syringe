using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories.MongoDB
{
	public class UserRepository : IUserRepository
	{
		private static readonly string COLLECTION_NAME = "Users";
		private readonly Configuration _configuration;
		private readonly MongoClient _mongoClient;
		private readonly IMongoDatabase _database;
		private readonly IMongoCollection<User> _collection;

		public UserRepository() : this(new Configuration())
		{
		}

		public UserRepository(Configuration configuration)
		{
			_configuration = configuration;
			_mongoClient = new MongoClient(_configuration.ConnectionString);
			_database = _mongoClient.GetDatabase(_configuration.DatabaseName);
			_collection = _database.GetCollection<User>(COLLECTION_NAME);
		}

		/// <summary>
		/// Adds a user, hashing the user password before hand.
		/// </summary>
		public void AddUser(User user)
		{
			user.Password = User.HashPassword(user.Password);
			_collection.InsertOneAsync(user);
		}

		/// <summary>
		/// Updates (replaces) a user in the database, hashing the password if it has changed.
		/// </summary>
		public void UpdateUser(User user, bool passwordHasChanged)
		{
			if (passwordHasChanged)
				user.Password = User.HashPassword(user.Password);

			_collection.ReplaceOneAsync(u => u.Id == user.Id, user);
		}

		/// <summary>
		/// Removes a user from the database, based on their id.
		/// </summary>
		public void DeleteUser(User user)
		{
			_collection.DeleteOneAsync(u => u.Id == user.Id);
		}

		/// <summary>
		/// Gets all users in the database.
		/// </summary>
		public IEnumerable<User> GetUsers()
		{
			return _collection.AsQueryable().ToList();
		}

		/// <summary>
		/// Gets all users that exist in the given team.
		/// </summary>
		public IEnumerable<User> GetUsersInTeam(Team team)
		{
			var filter = new FilterDefinitionBuilder<User>()
								.In(u => u.Id, team.UserIds);

			return _collection.Find(filter).ToListAsync().Result;
		}

		/// <summary>
		/// Removes all users from the database.
		/// </summary>
		public void Wipe()
		{
			_database.DropCollectionAsync(COLLECTION_NAME).Wait();
		}
	}
}
