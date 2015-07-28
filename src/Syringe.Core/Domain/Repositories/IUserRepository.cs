using System.Collections.Generic;
using Syringe.Core.Domain.Entities;

namespace Syringe.Core.Domain.Repositories
{
	public interface IUserRepository
	{
		void AddUser(User user);
		void UpdateUser(User user, bool passwordHasChanged);
		void DeleteUser(User user);

		IEnumerable<User> GetUsers();
	}
}
