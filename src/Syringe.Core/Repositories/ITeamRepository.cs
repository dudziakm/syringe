using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories
{
	public interface ITeamRepository
	{
		Task AddTeamAsync(Team team);
		Task UpdateTeamAsync(Team team);
		Task DeleteTeamAsync(Team team);
		Task AddUserToTeamAsync(Team team, User user);
		Task RemoveUserFromTeamAsync(Team team, User user);
		void Wipe();

		IEnumerable<Team> GetTeams();
		Team GetTeam(string name);
	}
}