using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		}

		private async Task<User> AddJohnDoeUserAsync()
		{
			User user = new User() { Id = Guid.NewGuid(), Firstname = "John", Lastname = "Doe", Email = "email@example.com" };
			UserRepository userRepository = CreateUserRepository();
			await userRepository.AddUserAsync(user);

			return user;
		}

		private async Task<User> AddJaneDoeUserAsync()
		{
			User user = new User() { Id = Guid.NewGuid(), Firstname = "Jane", Lastname = "Doe", Email = "jane@example.com" };
			UserRepository userRepository = CreateUserRepository();
			await userRepository.AddUserAsync(user);

			return user;
		}

		private async Task<Team> AddPowerRangersTeamAsync(TeamRepository teamRepository = null)
		{
			Team team = new Team()
			{
				Id = Guid.NewGuid(),
				Name = "Power Rangers"
			};

			if (teamRepository == null)
				teamRepository = CreateTeamRepository();

			await teamRepository.AddTeamAsync(team);

			return team;
		}

		[Test]
		public async Task AddTeam_should_store_the_team()
		{
			// Arrange
			TeamRepository teamRepository = CreateTeamRepository();

            // Act
			Team expectedTeam = await AddPowerRangersTeamAsync(teamRepository);

			// Assert
			IEnumerable<Team> teams = teamRepository.GetTeams();

			Assert.That(teams.Count(), Is.EqualTo(1));

			Team actualTeam = teams.FirstOrDefault();
			Assert.That(actualTeam.Id, Is.EqualTo(expectedTeam.Id));
			Assert.That(actualTeam.Name, Is.EqualTo(expectedTeam.Name));
			Assert.That(actualTeam.UserIds, Is.Not.Null);
		}

		[Test]
		public async Task UpdateTeam_should_store_the_updated_team_details()
		{
			// Arrange
			TeamRepository teamRepository = CreateTeamRepository();
			Team team = await AddPowerRangersTeamAsync(teamRepository);

			// Act
			IEnumerable<Team> teams = teamRepository.GetTeams();
			Team updatedTeam = teams.FirstOrDefault();
			team.Name = "Dad's Army";
			await teamRepository.UpdateTeamAsync(updatedTeam);

			// Assert
			teams = teamRepository.GetTeams();

			Team actualTeam = teams.FirstOrDefault();
			Assert.That(actualTeam.Id, Is.EqualTo(updatedTeam.Id));
			Assert.That(actualTeam.Name, Is.EqualTo(updatedTeam.Name));
		}

		[Test]
		public async Task DeleteTeam_should_remove_the_team()
		{
			// Arrange
			TeamRepository teamRepository = CreateTeamRepository();
			Team team = await AddPowerRangersTeamAsync(teamRepository);

			// Act
			await teamRepository.DeleteTeamAsync(team);

			// Assert
			IEnumerable<Team> teams = teamRepository.GetTeams();

			Assert.That(teams.Count(), Is.EqualTo(0));
		}

		[Test]
		public async Task GetTeam_should_return_team_and_be_case_insensitive()
		{
			// Arrange
			TeamRepository teamRepository = CreateTeamRepository();
			Team expectedTeam = await AddPowerRangersTeamAsync(teamRepository);

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
		public async Task GetTeams_should_return_all_teams()
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
			await teamRepository.AddTeamAsync(team1);
			await teamRepository.AddTeamAsync(team2);

			// Act
			IEnumerable<Team> teams = teamRepository.GetTeams();

			// Assert
			Assert.That(teams.Count(), Is.EqualTo(2));
		}

		[Test]
		public async Task AddUserToTeam_should_store_user_ids_for_the_team()
		{
			// Arrange
			User user1 = await AddJohnDoeUserAsync();
			User user2 = await AddJaneDoeUserAsync();

			TeamRepository teamRepository = CreateTeamRepository();
			Team team = await AddPowerRangersTeamAsync(teamRepository);

			// Act
			await teamRepository.AddUserToTeamAsync(team, user1);
			await teamRepository.AddUserToTeamAsync(team, user2);

			// Assert
			Team actualTeam = teamRepository.GetTeam(team.Name);
            Assert.That(actualTeam.UserIds.Count(), Is.EqualTo(2));
		}

		[Test]
		public async Task RemoveUserFromTeam_should_remove_user_id_from_the_team()
		{
			// Arrange
			User user1 = await AddJohnDoeUserAsync();
			User user2 = await AddJaneDoeUserAsync();

			Team team = await AddPowerRangersTeamAsync();

			TeamRepository teamRepository = CreateTeamRepository();
			await teamRepository.AddUserToTeamAsync(team, user1);
			await teamRepository.AddUserToTeamAsync(team, user2);

			// Act
			await teamRepository.RemoveUserFromTeamAsync(team, user1);

			// Assert
			Team actualTeam = teamRepository.GetTeam(team.Name);
			Assert.That(actualTeam, Is.Not.Null, "the team could not be found");
			Assert.That(actualTeam.UserIds.Count(), Is.EqualTo(1));
		}
	}
}