using System;
using Syringe.Core.Xml;

namespace Syringe.Core.Results.Writer
{
	public class ConsoleResultWriter : ResultWriterBase
	{
		protected override void WriteLine(string format, params object[] args)
		{
			Console.WriteLine(format, args);
		}
	}
}