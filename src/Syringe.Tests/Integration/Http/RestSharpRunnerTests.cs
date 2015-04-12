using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Http;

namespace Syringe.Tests.Integration.Http
{
	public class RestSharpRunnerTests
	{
		[Test]
		public void should_do_something()
		{
			// Arrange
			var runner = new RestSharpRunner(new Config());

			// Act
			runner.Run(@"Integration\wikipedia-example.xml");

			// Assert
		}
	}
}
