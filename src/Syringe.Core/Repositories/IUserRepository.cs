using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories
{
	public interface IUserRepository
	{
		Task AddUserAsync(User user);
		Task UpdateUserAsync(User user, bool passwordHasChanged);
		Task DeleteUserAsync(User user);
		void Wipe();

		IEnumerable<User> GetUsers();
		IEnumerable<User> GetUsersInTeam(Team team);
	}
}
