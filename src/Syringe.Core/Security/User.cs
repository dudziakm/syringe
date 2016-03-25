using System;
using System.Security.Cryptography;
using System.Text;
using Syringe.Core.TestCases.Configuration;

namespace Syringe.Core.Security
{
	public class User
	{
		public Guid Id { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }

		public override bool Equals(object obj)
		{
			User other = obj as User;
			return other != null && other.Id.Equals(Id);
		}

	    public override int GetHashCode()
	    {
	        return Id.GetHashCode();
	    }
	}
}