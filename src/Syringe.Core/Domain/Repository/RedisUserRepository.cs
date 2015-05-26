using System;
using System.Collections.Generic;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using Syringe.Core.Domain.Entities;

namespace Syringe.Core.Domain.Repository
{
	public class RedisUserRepository : IUserRepository, IDisposable
	{
		internal RedisClient RedisClient { get; set; }
		internal RedisTypedClient<User> RedisUserClient { get; set; }

		public RedisUserRepository() : this("localhost", 6379)
		{
		}

		public RedisUserRepository(string host, int port)
		{
			if (host == null) 
				throw new ArgumentNullException("host");

			if (port < 1)
				throw new ArgumentException("port is is invalid", "port");

			RedisClient = new RedisClient(host, port);
			RedisUserClient = new RedisTypedClient<User>(RedisClient);
		}

		public void AddUser(User user)
		{
			user.Password = User.HashPassword(user.Password);
			RedisUserClient.Store(user);
		}

		public void UpdateUser(User user, bool passwordHasChanged)
		{
			if (passwordHasChanged)
				user.Password = User.HashPassword(user.Password);

			RedisUserClient.Store(user);
		}

		public IEnumerable<User> GetUsers()
		{
			return RedisUserClient.GetAll();
		}

		public void DeleteUser(User user)
		{
			RedisUserClient.Delete(user);
		}

		public void Dispose()
		{
			if (RedisClient != null)
				RedisClient.Dispose();
		}
	}
}