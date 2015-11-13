using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Syringe.Core.Repositories.MongoDB;
using Syringe.Core.Security;

namespace Syringe.Tests.Integration.Repository.MongoDB
{
	public class UserRepositoryTests
	{
		private UserRepository CreateUserRepository()
		{
			return new UserRepository(new Configuration() { DatabaseName = "Syringe-Tests"});	
		}

		[SetUp]
		public void SetUp()
		{
			CreateUserRepository().Wipe();
		}

		[Test]
		public void AddUser_should_store_a_user()
		{
			// Arrange
			var expectedUser = new User()
			{
				Id = Guid.NewGuid(),
				Email = "nano@nano.com",
				Firstname = "Nano",
				Lastname = "Smith",
				Password = "This will be a hashed password"
			};

			UserRepository repository = CreateUserRepository();

			// Act
			repository.AddUser(expectedUser);

			// Assert
			IEnumerable<User> users = repository.GetUsers();

			Assert.That(users.Count(), Is.EqualTo(1));

			User actualUser = users.FirstOrDefault();
			Assert.That(actualUser.Id, Is.EqualTo(expectedUser.Id));
			Assert.That(actualUser.Email, Is.EqualTo(expectedUser.Email));
			Assert.That(actualUser.Firstname, Is.EqualTo(expectedUser.Firstname));
			Assert.That(actualUser.Lastname, Is.EqualTo(expectedUser.Lastname));
			Assert.That(actualUser.Password, Is.EqualTo(expectedUser.Password)); // password property is updated
		}

		[Test]
		public void AddUser_should_hash_the_password()
		{
			// Arrange
			string plainTextPassword = "This will be a hashed password";
			var expectedUser = new User()
			{
				Id = Guid.NewGuid(),
				Email = "nano@nano.com",
				Password = plainTextPassword
			};

			UserRepository repository = CreateUserRepository();

			// Act
			repository.AddUser(expectedUser);

			// Assert
			IEnumerable<User> users = repository.GetUsers();
			User actualUser = users.FirstOrDefault();

			Assert.That(actualUser, Is.Not.Null);
			Assert.That(actualUser.Password, Is.Not.EqualTo(plainTextPassword));
		}

		[Test]
		public void UpdateUser_should_store_the_updated_details()
		{
			// Arrange
			var newUser = new User()
			{
				Id = Guid.NewGuid(),
				Email = "nano@nano.com",
				Firstname = "Nano",
				Lastname = "Smith",
				Password = "This will be a hashed password"
			};

			UserRepository repository = CreateUserRepository();
			repository.AddUser(newUser);
			
			// Act
			IEnumerable<User> users = repository.GetUsers();
			User userToUpdate = users.FirstOrDefault();
			userToUpdate.Firstname = "Updated firstname";
			userToUpdate.Lastname = "Updated lastname";
			userToUpdate.Email = "Updated email";

			repository.UpdateUser(userToUpdate, false);

			// Assert
			users = repository.GetUsers();
			User actualUser = users.FirstOrDefault();

			Assert.That(actualUser.Id, Is.EqualTo(userToUpdate.Id));
			Assert.That(actualUser.Email, Is.EqualTo(userToUpdate.Email));
			Assert.That(actualUser.Firstname, Is.EqualTo(userToUpdate.Firstname));
			Assert.That(actualUser.Lastname, Is.EqualTo(userToUpdate.Lastname));
			Assert.That(actualUser.Password, Is.EqualTo(userToUpdate.Password));
		}

		[Test]
		public void UpdateUser_should_hash_the_new_password_when_set()
		{
			// Arrange
			string newPlainTextPassword = "My new password";
			var newUser = new User()
			{
				Id = Guid.NewGuid(),
				Email = "nano@nano.com",
				Firstname = "Nano",
				Lastname = "Smith",
				Password = "This will be a hashed password"
			};

			UserRepository repository = CreateUserRepository();
			repository.AddUser(newUser);

			IEnumerable<User> users = repository.GetUsers();

			// Act
			User updatedUser = users.FirstOrDefault();
			updatedUser.Password = newPlainTextPassword;

			repository.UpdateUser(updatedUser, true);

			// Assert
			users = repository.GetUsers();
			User userFromRepo = users.FirstOrDefault();
			Assert.That(userFromRepo.Password, Is.Not.EqualTo(newPlainTextPassword));
		}

		[Test]
		public void DeleteUser_should_remove_the_user()
		{
			// Arrange
			string plainTextPassword = "This will be a hashed password";
			var user = new User()
			{
				Id = Guid.NewGuid(),
				Email = "nano@nano.com",
				Password = plainTextPassword
			};

			UserRepository repository = CreateUserRepository();
			repository.AddUser(user);

			// Act
			repository.DeleteUser(user);

			// Assert
			IEnumerable<User> users = repository.GetUsers();
			Assert.That(users.Count(), Is.EqualTo(0));
		}

		[Test]
		public void GetUsersInTeam_should_store_a_user()
		{
			// Arrange
			var user1 = new User()
			{
				Id = Guid.NewGuid(),
				Email = "nano1@nano.com",
				Firstname = "Nano1",
				Lastname = "Smith1",
				Password = "pass"
			};

			var user2 = new User()
			{
				Id = Guid.NewGuid(),
				Email = "nano2@nano.com",
				Firstname = "Nano2",
				Lastname = "Smith2",
				Password = "pass2"
			};

			var user3 = new User()
			{
				Id = Guid.NewGuid(),
				Email = "nano3@nano.com",
				Firstname = "Nano3",
				Lastname = "Smith3",
				Password = "pass3"
			};

			UserRepository repository = CreateUserRepository();
			repository.AddUser(user1);
			repository.AddUser(user2);
			repository.AddUser(user3);

			var team = new Team();
			team.UserIds.Add(user1.Id);
			team.UserIds.Add(user2.Id);

			// Act

			// Assert
			IEnumerable<User> users = repository.GetUsersInTeam(team);

			Assert.That(users.Count(), Is.EqualTo(2));
			Assert.That(users, Contains.Item(user1));
			Assert.That(users, Contains.Item(user2));
		}
	}
}