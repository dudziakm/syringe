using System;
using System.Text;

namespace Syringe.Core.Domain.Entities
{
	/// <summary>
	/// Generates a random 16 character string for a hashed password salt.
	/// </summary>
	public class Salt
	{
		private static readonly Random _random = new Random();

		/// <summary>
		/// The salt value.
		/// </summary>
		public string Value { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Salt"/> class, generating a new salt value.
		/// </summary>
		public Salt()
		{
			StringBuilder builder = new StringBuilder(16);
			for (int i = 0; i < 16; i++)
			{
				builder.Append((char) _random.Next(33, 126));
			}

			Value = builder.ToString();
		}

		public static implicit operator string(Salt salt)
		{
			return salt.Value;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
