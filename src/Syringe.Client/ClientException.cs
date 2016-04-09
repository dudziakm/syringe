using System;

namespace Syringe.Client
{
	public class ClientException : Exception
	{
		public ClientException(string message, string name) : base(message)
		{
		}

		public ClientException(string message, params object[] args) : base(string.Format(message, args))
		{
		}
	}
}