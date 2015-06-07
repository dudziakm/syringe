using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;
using Syringe.Core.Runner;
using Syringe.Core.Xml;
using Syringe.Core.Xml.Reader;
using YamlDotNet.Serialization;

namespace Syringe.Tests.Integration.Xml
{
	public class TestCaseRunnerTests
	{
		public static string XmlExamplesFolder = "Syringe.Tests.Integration.Xml.XmlExamples.Runner.";

		[Test]
		public void should_parse_responses()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var httpLogWriter = new HttpLogWriter(new StringWriter(stringBuilder));
			var httpClient = new HttpClient(httpLogWriter, new RestClient());

			var config = new Config();

			string xml = TestHelpers.ReadEmbeddedFile("parsedresponses.xml", XmlExamplesFolder);
			var stringReader = new StringReader(xml);
			var reader = new TestCaseReader();
			var caseCollection = reader.Read(stringReader);
			var runner = new TestSessionRunner(config, httpClient, new ConsoleResultWriter());

			// Act
			TestCaseSession result = runner.Run(caseCollection);

			// Assert
			DumpAsYaml(result);
		}

		[Test]
		public void should_do_something()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var httpLogWriter = new HttpLogWriter(new StringWriter(stringBuilder));
			var httpClient = new HttpClient(httpLogWriter, new RestClient());

			var config = new Config();

			string xml = TestHelpers.ReadEmbeddedFile("wikipedia-simple.xml", XmlExamplesFolder);
			var stringReader = new StringReader(xml);
			var reader = new LegacyTestCaseReader();
			var caseCollection = reader.Read(stringReader);

			var runner = new TestSessionRunner(config, httpClient, new ConsoleResultWriter());

			// Act
			runner.Run(caseCollection);

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
			var httpClient = new HttpClient(new HttpLogWriter(stringWriter), new RestClient());

			var resultWriter = new TextWriterResultWriter(stringWriter);

			string xml = TestHelpers.ReadEmbeddedFile("roadkill-login.xml", XmlExamplesFolder);
			var stringReader = new StringReader(xml);
			var reader = new TestCaseReader();

			var runner = new TestSessionRunner(config, httpClient, resultWriter);
			var caseCollection = reader.Read(stringReader);

			// Act
			TestCaseSession session = runner.Run(caseCollection);

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
