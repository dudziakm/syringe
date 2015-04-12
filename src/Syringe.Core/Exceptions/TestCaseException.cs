using System;

namespace Syringe.Core.Exceptions
{
	public class TestCaseException : Exception
	{
		public TestCaseException(string message, string name) : base(message)
		{
		}

		public TestCaseException(string message, params object[] args)
			: base(string.Format(message, args))
		{
		}
	}
}
