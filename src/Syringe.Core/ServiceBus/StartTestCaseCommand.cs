using System;
using NServiceBus;

namespace Syringe.Core.ServiceBus
{
	public class StartTestCaseCommand : ICommand
	{
		public Guid Id { get; set; }
		public string TestCaseFilename { get; set; }
	}
}
