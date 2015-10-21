using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Syringe.Core.Repositories.MongoDB;
using Syringe.Core.Security;

namespace Syringe.Tests.Integration.Repository.MongoDB
{
	public class TeamRepositoryTests
	{
		private TeamRepository CreateTeamRepository()
		{
			return new TeamRepository(new Configuration() { DatabaseName = "Syringe-Tests" });
		}

		private UserRepository CreateUserRepository()
		{
			return new UserRepository(new Configuration() { DatabaseName = "Syringe-Tests" });
		}

		[SetUp]
		public void SetUp()
		{
			CreateUserRepository().Wipe();
			CreateTeamRepository().Wipe();
			SetUpFixture.WaitForDatabaseWipe();
		}

		private User AddJohnDoeUser()
		{
			User user = new User() { Id = Guid.NewGuid(), Firstname = "John", Lastname = "Doe", Email = "email@example.com" };
			UserRepository userRepository = CreateUserRepository();
			userRepository.AddUser(user);

			return user;
		}

		private User AddJaneDoeUser()
		{
			User user = new User() { Id = Guid.NewGuid(), Firstname = "Jane", Lastname = "Doe", Email = "jane@example.com" };
			UserRepository userRepository = CreateUserRepository();
			userRepository.AddUser(user);

			return user;
		}

		private Team AddPowerRangersTeam(TeamRepository teamRepository = null)
		{
			Team team = new Team()
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
			TeamRepository teamRepository = CreateTeamRepository();
			Team expectedTeam = AddPowerRangersTeam(teamRepository);

			// Act
			teamRepository.AddTeam(expectedTeam);

			// Assert
			IEnumerable<Team> teams = teamRepository.GetTeams();

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
			TeamRepository teamRepository = CreateTeamRepository();
			Team team = AddPowerRangersTeam(teamRepository);

			// Act
			IEnumerable<Team> teams = teamRepository.GetTeams();
			Team updatedTeam = teams.FirstOrDefault();
			team.Name = "Dad's Army";
			teamRepository.UpdateTeam(updatedTeam);

			// Assert
			teams = teamRepository.GetTeams();

			Team actualTeam = teams.FirstOrDefault();
			Assert.That(actualTeam.Id, Is.EqualTo(updatedTeam.Id));
			Assert.That(actualTeam.Name, Is.EqualTo(updatedTeam.Name));
		}

		[Test]
		public void DeleteTeam_should_remove_the_team()
		{
			// Arrange
			TeamRepository teamRepository = CreateTeamRepository();
			Team team = AddPowerRangersTeam(teamRepository);

			// Act
			teamRepository.Delete(team);

			// Assert
			IEnumerable<Team> teams = teamRepository.GetTeams();

			Assert.That(teams.Count(), Is.EqualTo(0));
		}

		[Test]
		public void GetTeam_should_return_team_and_be_case_insensitive()
		{
			// Arrange
			TeamRepository teamRepository = CreateTeamRepository();
			Team expectedTeam = AddPowerRangersTeam(teamRepository);

			// Act
			Team actualTeam = teamRepository.GetTeam("POWER rangers");

			// Assert
			Assert.That(actualTeam, Is.Not.Null, "the team could not be found");
			Assert.That(actualTeam.Id, Is.EqualTo(expectedTeam.Id));
			Assert.That(actualTeam.Name, Is.EqualTo(expectedTeam.Name));
			Assert.That(actualTeam.UserIds, Is.Not.Null);
		}

		[Test]
		public void GetTeam_should_return_null_when_the_team_isnt_found()
		{
			// Arrange
			TeamRepository teamRepository = CreateTeamRepository();

			// Act
			Team actualTeam = teamRepository.GetTeam("foo fighters");

			// Assert
			Assert.That(actualTeam, Is.Null);
		}

		[Test]
		public void GetTeams_should_return_all_teams()
		{
			// Arrange
			Team team1 = new Team()
			{
				Id = Guid.NewGuid(),
				Name = "Power Rangers"
			};

			Team team2 = new Team()
			{
				Id = Guid.NewGuid(),
				Name = "Twilight Sparkles"
			};

			TeamRepository teamRepository = CreateTeamRepository();
			teamRepository.AddTeam(team1);
			teamRepository.AddTeam(team2);

			// Act
			IEnumerable<Team> teams = teamRepository.GetTeams();

			// Assert
			Assert.That(teams.Count(), Is.EqualTo(2));
		}

		[Test]
		public void AddUserToTeam_should_store_user_ids_for_the_team()
		{
			// Arrange
			User user1 = AddJohnDoeUser();
			User user2 = AddJaneDoeUser();

			TeamRepository teamRepository = CreateTeamRepository();
			Team team = AddPowerRangersTeam(teamRepository);

			// Act
			teamRepository.AddUserToTeam(team, user1);
			teamRepository.AddUserToTeam(team, user2);

			// Assert
			Team actualTeam = teamRepository.GetTeam(team.Name);
            Assert.That(actualTeam.UserIds.Count(), Is.EqualTo(2));
		}

		[Test]
		public void RemoveUserFromTeam_should_remove_user_id_from_the_team()
		{
			// Arrange
			User user1 = AddJohnDoeUser();
			User user2 = AddJaneDoeUser();

			Team team = AddPowerRangersTeam();

			TeamRepository teamRepository = CreateTeamRepository();
			teamRepository.AddUserToTeam(team, user1);
			teamRepository.AddUserToTeam(team, user2);

			// Act
			teamRepository.RemoveUserFromTeam(team, user1);

			// Assert
			Team actualTeam = teamRepository.GetTeam(team.Name);
			Assert.That(actualTeam, Is.Not.Null, "the team could not be found");
			Assert.That(actualTeam.UserIds.Count(), Is.EqualTo(1));
		}
	}
}