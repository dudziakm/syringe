using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;
using Syringe.Core.Runner;
using Syringe.Core.Xml;
using YamlDotNet.Serialization;

namespace Syringe.Tests.Integration
{
	public class TestCaseRunnerTests
	{
		[Test]
		public void should_do_something()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var httpLogWriter = new HttpLogWriter(new StringWriter(stringBuilder));
			var restSharpClient = new RestSharpClient(httpLogWriter);

			var config = new Config();
			var runner = new TestSessionRunner(config, restSharpClient, new ConsoleResultWriter());

			string xml = File.ReadAllText(Path.Combine("Integration", "wikipedia-simple.xml"));
			var stringReader = new StringReader(xml);
			var reader = new LegacyTestCaseReader(stringReader);

			// Act
			runner.Run(reader);

			// Assert
			Console.WriteLine(stringBuilder);
		}

		[Test]
		public void should_post()
		{
			// Arrange
			//_streamWriter = new StreamWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
			var stringBuilder = new StringBuilder();
			var stringWriter = new StringWriter(stringBuilder);
			
			var config = new Config();
			config.GlobalHttpLog = LogType.All;
			var restSharpClient = new RestSharpClient(new HttpLogWriter(stringWriter));

			var httpLogWriter = new HttpLogWriter(stringWriter);
			var resultWriter = new TextWriterResultWriter(stringWriter);

			var runner = new TestSessionRunner(config, restSharpClient, resultWriter);

			string xml = File.ReadAllText(Path.Combine("Integration", "roadkill-login.xml"));
			var stringReader = new StringReader(xml);
			var reader = new TestCaseReader(stringReader);

			// Act
			TestCaseSession session = runner.Run(reader);

			// Assert
			DumpAsXml(session);
			DumpAsYaml(session);

			Console.WriteLine(stringBuilder);
		}

		private static void DumpAsYaml(TestCaseSession session)
		{
			var stringBuilder = new StringBuilder();
			var serializer = new Serializer();
			serializer.Serialize(new IndentedTextWriter(new StringWriter(stringBuilder)), session);
			Console.WriteLine(stringBuilder);
		}

		private static void DumpAsXml(TestCaseSession session)
		{
			var stringBuilder = new StringBuilder();
			var serializer = new XmlSerializer(typeof (TestCaseSession));
			serializer.Serialize(new StringWriter(stringBuilder), session);

			Console.WriteLine(stringBuilder);
		}
	}
}
