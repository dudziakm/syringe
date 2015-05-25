using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Core.Domain;
using Syringe.Core.Repository;

namespace Syringe.Tests.Integration.Repository
{
	public class RedisUserRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			var repository = new RedisUserRepository();
			repository.RedisUserClient.DeleteAll();
		}

		[Test]
		public void should_add_a_user()
		{
			// Arrange
			var repository = new RedisUserRepository();

			// Act
			repository.AddUser(new User()
			{
				Id = Guid.NewGuid(),
				Email = "nano@nano.com"
			});

			IEnumerable<User> users = repository.GetUsers();

			// Assert
			Assert.That(users.Count(), Is.EqualTo(1));
		}
	}
}