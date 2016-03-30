using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Http;
using Syringe.Core.Repositories;
using Syringe.Core.Runner;
using Syringe.Core.Tests.Results;
using Syringe.Core.Xml.Reader;
using Syringe.Tests.StubsMocks;
using YamlDotNet.Serialization;

namespace Syringe.Tests.Integration.Xml
{
	public class TestCaseRunnerTests
	{
		public static string XmlExamplesFolder = typeof(TestCaseRunnerTests).Namespace + ".XmlExamples.Runner.";

		private ITestCaseSessionRepository GetRepository()
		{
			return new TestCaseSessionRepositoryMock();
		}

		[Test]
		public async Task should_parse_responses()
		{
			// Arrange
			var httpClient = new HttpClient(new RestClient());

			string xml = TestHelpers.ReadEmbeddedFile("capturedvariables.xml", XmlExamplesFolder);
			var stringReader = new StringReader(xml);
			var reader = new TestFileReader();
			var caseCollection = reader.Read(stringReader);
			var runner = new TestFileRunner(httpClient, GetRepository());

			// Act
			TestFileResult result = await runner.RunAsync(caseCollection);

			// Assert
			DumpAsYaml(result);
		}

		[Test]
		public async Task should_post()
		{
			// Arrange		
			var httpClient = new HttpClient(new RestClient());

			string xml = TestHelpers.ReadEmbeddedFile("roadkill-login.xml", XmlExamplesFolder);
			var stringReader = new StringReader(xml);
			var reader = new TestFileReader();

			var runner = new TestFileRunner(httpClient, GetRepository());
			var caseCollection = reader.Read(stringReader);

			// Act
			TestFileResult session = await runner.RunAsync(caseCollection);

			// Assert
			DumpAsXml(session);
			DumpAsYaml(session);
		}

		private static void DumpAsYaml(TestFileResult session)
		{
			// Messing around with YAML, that's the only reason for this (and it also looks nicer than XML)
			var stringBuilder = new StringBuilder();
			var serializer = new Serializer();
			serializer.Serialize(new IndentedTextWriter(new StringWriter(stringBuilder)), session);
		}

		private static void DumpAsXml(TestFileResult session)
		{
			var stringBuilder = new StringBuilder();
			var serializer = new XmlSerializer(typeof (TestFileResult));
			serializer.Serialize(new StringWriter(stringBuilder), session);
		}
	}
}
