using System;
using NServiceBus;

namespace Syringe.Core.ServiceBus
{
	public class TestCaseStartedEvent : IEvent
	{
		public Guid Id { get; set; }
	}
}