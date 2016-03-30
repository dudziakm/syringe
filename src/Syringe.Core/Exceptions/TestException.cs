using System;

namespace Syringe.Core.Exceptions
{
	public class TestException : Exception
	{
		public TestException(string message, string name) : base(message)
		{
		}

		public TestException(string message, params object[] args)
			: base(string.Format(message, args))
		{
		}
	}
}
