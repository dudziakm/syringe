using System.Collections.Generic;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories
{
	public interface ITeamRepository
	{
		void AddTeam(Team team);
		void UpdateTeam(Team team);
		void Delete(Team team);
		void AddUserToTeam(Team team, User user);
		void RemoveUserFromTeam(Team team, User user);
		void Wipe();

		IEnumerable<Team> GetTeams();
		Team GetTeam(string name);
	}
}