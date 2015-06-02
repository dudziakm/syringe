using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syringe.Core.Security
{
	public class UserContext : IUserContext
	{
		// temporary
		public string TeamName
		{
			get { return "teamname"; }
		}

		public string Username
		{
			get { return "tim.sherwood"; }
		}
	}
}
