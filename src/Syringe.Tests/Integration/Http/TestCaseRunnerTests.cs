using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Runner;
using Syringe.Core.Xml;
using Syringe.Tests.Unit.StubsMocks;

namespace Syringe.Tests.Integration.Http
{
	public class TestCaseRunnerTests
	{
		[Test]
		public void should_do_something()
		{
			// Arrange
			var config = new Config();
			var restSharpClient = new RestSharpClient();
			var stringBuilder = new StringBuilder();
			var httpLogWriter = new HttpLogWriter(new StringWriter(stringBuilder));

			var runner = new TestSessionRunner(config, restSharpClient, httpLogWriter);

			// Act
			var reader = new LegacyTestCaseReader();
			string xml = File.ReadAllText(Path.Combine("Integration", "wikipedia-simple.xml"));
			var stringReader = new StringReader(xml);
			runner.Run(reader,stringReader);

			// Assert
			Console.WriteLine(stringBuilder);
		}

		[Test]
		public void should_post()
		{
			// Arrange
			var config = new Config();
			config.GlobalHttpLog = LogType.All;
			var restSharpClient = new RestSharpClient();
			var stringBuilder = new StringBuilder();
			var httpLogWriter = new HttpLogWriter(new StringWriter(stringBuilder));

			var runner = new TestSessionRunner(config, restSharpClient, httpLogWriter);

			// Act
			var reader = new TestCaseReader();
			string xml = File.ReadAllText(Path.Combine("Integration", "roadkill-login.xml"));
			var stringReader = new StringReader(xml);
			runner.Run(reader, stringReader);

			// Assert
			Console.WriteLine("==============================");
			Console.WriteLine(stringBuilder);
		}
	}
}
