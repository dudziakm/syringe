using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Syringe.Core.Repositories.Redis;
using Syringe.Core.Security;

namespace Syringe.Tests.Integration.Repository
{
	public class RedisTeamRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			var userRepository = CreateUserRepository();
			userRepository.RedisUserClient.DeleteAll();

			var teamRepository = CreateTeamRepository();
			teamRepository.RedisTeamClient.DeleteAll();
		}

		private RedisTeamRepository CreateTeamRepository()
		{
			return new RedisTeamRepository();
		}

		private RedisUserRepository CreateUserRepository()
		{
			return new RedisUserRepository();
		}

		private User AddJohnDoe()
		{
			var user = new User() { Id = Guid.NewGuid(), Firstname = "John", Lastname = "Doe", Email = "email@example.com" };
			using (var userRepository = CreateUserRepository())
			{
				userRepository.AddUser(user);
			}

			return user;
		}

		private User AddJaneDoe()
		{
			var user = new User() { Id = Guid.NewGuid(), Firstname = "Jane", Lastname = "Doe", Email = "jane@example.com" };
			using (var userRepository = CreateUserRepository())
			{
				userRepository.AddUser(user);
			}

			return user;
		}

		private Team AddPowerRangers()
		{
			var team = new Team()
			{
				Id = Guid.NewGuid(),
				Name = "Power Rangers"
			};

			using (var repository = CreateTeamRepository())
			{
				repository.AddTeam(team);
			}

			return team;
		}

		[Test]
		public void AddTeam_should_store_the_team()
		{
			// Arrange
			var expectedTeam = new Team()
			{
				Id = Guid.NewGuid(),
				Name = "Power Rangers"
			};

			var repository = CreateTeamRepository();

			// Act
			repository.AddTeam(expectedTeam);

			// Assert
			IEnumerable<Team> teams = repository.GetTeams();

			Assert.That(teams.Count(), Is.EqualTo(1));

			Team actualTeam = teams.FirstOrDefault();
			Assert.That(actualTeam.Id, Is.EqualTo(expectedTeam.Id));
			Assert.That(actualTeam.Name, Is.EqualTo(expectedTeam.Name));
			Assert.That(actualTeam.UserIds, Is.Not.Null);
		}

		[Test]
		public void UpdateTeam_should_store_the_updated_team_details()
		{
			// Arrange
			var team = AddPowerRangers();
			var repository = CreateTeamRepository();

			// Act
			IEnumerable<Team> teams = repository.GetTeams();
			Team updatedTeam = teams.FirstOrDefault();
			updatedTeam.Name = "Dad's Army";
			repository.UpdateTeam(updatedTeam);

			// Assert
			teams = repository.GetTeams();

			Team actualTeam = teams.FirstOrDefault();
			Assert.That(actualTeam.Id, Is.EqualTo(updatedTeam.Id));
			Assert.That(actualTeam.Name, Is.EqualTo(updatedTeam.Name));
		}

		[Test]
		public void DeleteTeam_should_remove_the_team()
		{
			// Arrange
			var team = AddPowerRangers();
			var repository = CreateTeamRepository();

			// Act
			repository.Delete(team);

			// Assert
			IEnumerable<Team> teams = repository.GetTeams();

			Assert.That(teams.Count(), Is.EqualTo(0));
		}

		[Test]
		public void GetTeam_should_return_team_and_be_case_insensitive()
		{
			// Arrange
			var expectedTeam = AddPowerRangers();
			var repository = CreateTeamRepository();

			// Act
			Team actualTeam = repository.GetTeam("POWER rangers");

			// Assert
			Assert.That(actualTeam.Id, Is.EqualTo(expectedTeam.Id));
			Assert.That(actualTeam.Name, Is.EqualTo(expectedTeam.Name));
			Assert.That(actualTeam.UserIds, Is.Not.Null);
		}

		[Test]
		public void GetTeam_should_return_null_when_the_team_isnt_found()
		{
			// Arrange
			var repository = CreateTeamRepository();

			// Act
			Team actualTeam = repository.GetTeam("foo fighters");

			// Assert
			Assert.That(actualTeam, Is.Null);
		}

		[Test]
		public void GetTeams_should_return_all_teams()
		{
			// Arrange
			var team1 = new Team()
			{
				Id = Guid.NewGuid(),
				Name = "Power Rangers"
			};

			var team2 = new Team()
			{
				Id = Guid.NewGuid(),
				Name = "Twilight Sparkles"
			};

			var repository = CreateTeamRepository();
			repository.AddTeam(team1);
			repository.AddTeam(team2);

			// Act
			IEnumerable<Team> teams = repository.GetTeams();

			// Assert
			Assert.That(teams.Count(), Is.EqualTo(2));
		}

		[Test]
		public void AddUserToTeam_should_store_user_ids_for_the_team()
		{
			// Arrange
			var user1 = AddJohnDoe();
			var user2 = AddJaneDoe();

			var team = AddPowerRangers();
			var teamTepository = CreateTeamRepository();

			// Act
			teamTepository.AddUserToTeam(team, user1);
			teamTepository.AddUserToTeam(team, user2);

			// Assert
			IEnumerable<User> users = teamTepository.GetUsersInTeam(team);
			Assert.That(users.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetUsersInTeam_should_return_the_correct_users()
		{
			// Arrange
			var user1 = AddJohnDoe();
			var user2 = AddJaneDoe();

			var team = AddPowerRangers();
			var teamTepository = CreateTeamRepository();

			// Act
			teamTepository.AddUserToTeam(team, user1);
			teamTepository.AddUserToTeam(team, user2);

			// Assert
			IEnumerable<User> users = teamTepository.GetUsersInTeam(team);
			Assert.That(users.Count(), Is.EqualTo(2));

			User actualUser1 = users.FirstOrDefault(x => x.Id == user1.Id);
			Assert.That(actualUser1, Is.Not.Null);

			User actualUser2 = users.FirstOrDefault(x => x.Id == user2.Id);
			Assert.That(actualUser2, Is.Not.Null);
		}

		[Test]
		public void RemoveUserFromTeam_should_remove_user_id_from_the_team()
		{
			// Arrange
			var user1 = AddJohnDoe();
			var user2 = AddJaneDoe();

			var team = AddPowerRangers();

			var teamTepository = CreateTeamRepository();
			teamTepository.AddUserToTeam(team, user1);
			teamTepository.AddUserToTeam(team, user2);

			// Act
			teamTepository.RemoveUserFromTeam(team, user1);

			// Assert
			IEnumerable<User> users = teamTepository.GetUsersInTeam(team);
			Assert.That(users.Count(), Is.EqualTo(1));
		}
	}
}