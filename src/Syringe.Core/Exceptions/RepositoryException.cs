using System;

namespace Syringe.Core.Domain.Repository
{
	public class RepositoryException : Exception
	{
		public RepositoryException(string format, params object[] args) : base(string.Format(format, args))
		{
		}
	}
}