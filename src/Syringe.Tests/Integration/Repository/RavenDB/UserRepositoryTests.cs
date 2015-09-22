using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Syringe.Core.Repositories.RavenDB;
using Syringe.Core.Schedule;
using Syringe.Core.Security;

namespace Syringe.Tests.Integration.Repository.RavenDB
{
	public class UserRepositoryTests
	{
		private RavenDbUserRepository CreateUserRepository()
		{
			return new RavenDbUserRepository(RavenDbTestSetup.DocumentStore);	
		}

		[SetUp]
		public void SetUp()
		{
			RavenDbTestSetup.ClearDocuments<User>();
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

			var repository = CreateUserRepository();

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

			var repository = CreateUserRepository();

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

			var repository = CreateUserRepository();
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

			var repository = CreateUserRepository();
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

			var repository = CreateUserRepository();
			repository.AddUser(user);

			// Act
			repository.DeleteUser(user);

			// Assert
			IEnumerable<User> users = repository.GetUsers();
			Assert.That(users.Count(), Is.EqualTo(0));
		}
	}
}