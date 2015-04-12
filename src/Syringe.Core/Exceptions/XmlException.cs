using System;

namespace Syringe.Core.Exceptions
{
	public class XmlException : Exception
	{
		public XmlException(string message, params object[] args) : base(string.Format(message, args))
		{
		}
	}
}
