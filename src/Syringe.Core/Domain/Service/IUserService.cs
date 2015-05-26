using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syringe.Core.Domain.Service
{
	public interface IUserService
	{
		bool Authenticate(string email, string password);
	}
}