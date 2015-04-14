using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Http;
using Syringe.Tests.Unit.StubsMocks;

namespace Syringe.Tests.Integration.Http
{
	public class RestSharpRunnerTests
	{
		[Test]
		public void should_do_something()
		{
			// Arrange
			var runner = new RestSharpRunner(new Config(), new HttpLogWriter(new TextWriterFactoryMock(new StringBuilder())));

			// Act
			runner.Run(Path.Combine("Integration" ,"wikipedia-example.xml"));

			// Assert
		}
	}
}
