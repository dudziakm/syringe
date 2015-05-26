using System;
using System.Security.Cryptography;
using System.Text;
using Syringe.Core.Configuration;

namespace Syringe.Core.Domain.Entities
{
	public class User
	{
		public Guid Id { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public Config Config { get; set; }

		/// <summary>
		/// Hashes a combination of the password and salt using SHA512.
		/// </summary>
		public static string HashPassword(string password)
		{
			var salt = new Salt();
			SHA512 sha = new SHA512Managed();
			byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(password + salt));

			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in hash)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}

			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			User other = obj as User;
			return other != null && other.Id.Equals(Id);
		}
	}
}