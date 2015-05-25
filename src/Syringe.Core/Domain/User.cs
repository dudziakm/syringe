using System;

namespace Syringe.Core.Domain
{
	public class User
	{
		public Guid Id { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
	}
}