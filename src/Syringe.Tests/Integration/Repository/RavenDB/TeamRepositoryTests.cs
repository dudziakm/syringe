using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Syringe.Core.Repositories.RavenDB;
using Syringe.Core.Repositories.Redis;
using Syringe.Core.Schedule;
using Syringe.Core.Security;

namespace Syringe.Tests.Integration.Repository.RavenDB
{
	public class TeamRepositoryTests
	{
		private RavenDbTeamRepository CreateTeamRepository()
		{
			return new RavenDbTeamRepository(RavenDbTestSetup.DocumentStore);
		}

		private RavenDbUserRepository CreateUserRepository()
		{
			return new RavenDbUserRepository(RavenDbTestSetup.DocumentStore);
		}

		[SetUp]
		public void SetUp()
		{
			RavenDbTestSetup.ClearDocuments<User>();
			RavenDbTestSetup.ClearDocuments<Team>();
		}

		private User AddJohnDoeUser()
		{
			var user = new User() { Id = Guid.NewGuid(), Firstname = "John", Lastname = "Doe", Email = "email@example.com" };
			var userRepository = CreateUserRepository();
			userRepository.AddUser(user);

			return user;
		}

		private User AddJaneDoeUser()
		{
			var user = new User() { Id = Guid.NewGuid(), Firstname = "Jane", Lastname = "Doe", Email = "jane@example.com" };
			var userRepository = CreateUserRepository();
			userRepository.AddUser(user);

			return user;
		}

		private Team AddPowerRangersTeam(RavenDbTeamRepository teamRepository = null)
		{
			var team = new Team()
			{
				Id = Guid.NewGuid(),
				Name = "Power Rangers"
			};

			if (teamRepository == null)
				teamRepository = CreateTeamRepository();

			teamRepository.AddTeam(team);

			return team;
		}

		[Test]
		public void AddTeam_should_store_the_team()
		{
			// Arrange
			var repository = CreateTeamRepository();
			Team expectedTeam = AddPowerRangersTeam(repository);

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
			var repository = CreateTeamRepository();
			var team = AddPowerRangersTeam(repository);

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
			var repository = CreateTeamRepository();
			var team = AddPowerRangersTeam(repository);

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
			var repository = CreateTeamRepository();
			var expectedTeam = AddPowerRangersTeam(repository);

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
			var user1 = AddJohnDoeUser();
			var user2 = AddJaneDoeUser();

			var teamTepository = CreateTeamRepository();
			var team = AddPowerRangersTeam(teamTepository);

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
			var user1 = AddJohnDoeUser();
			var user2 = AddJaneDoeUser();

			var teamRepository = CreateTeamRepository();
			var team = AddPowerRangersTeam(teamRepository);

			// Act
			teamRepository.AddUserToTeam(team, user1);
			teamRepository.AddUserToTeam(team, user2);

			// Assert
			IEnumerable<User> users = teamRepository.GetUsersInTeam(team);
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
			var user1 = AddJohnDoeUser();
			var user2 = AddJaneDoeUser();

			var team = AddPowerRangersTeam();

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