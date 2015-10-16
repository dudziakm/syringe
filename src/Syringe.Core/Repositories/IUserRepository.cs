using System.Collections.Generic;
using Syringe.Core.Security;

namespace Syringe.Core.Repositories
{
	public interface IUserRepository
	{
		void AddUser(User user);
		void UpdateUser(User user, bool passwordHasChanged);
		void DeleteUser(User user);
		void Wipe();

		IEnumerable<User> GetUsers();
		IEnumerable<User> GetUsersInTeam(Team team);
	}
}
