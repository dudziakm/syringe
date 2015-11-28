using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Http;
using Syringe.Core.Repositories;
using Syringe.Core.Runner;
using Syringe.Core.TestCases.Configuration;
using Syringe.Core.Xml.Reader;
using Syringe.Tests.StubsMocks;

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
		public async Task should_run_case()
		{
			// Arrange
			var httpClient = new HttpClient(new RestClient());

			var config = new Config();

			string xml = TestHelpers.ReadEmbeddedFile("wikipedia-simple.xml", XmlExamplesFolder);
			var stringReader = new StringReader(xml);
			var reader = new LegacyTestCaseReader();
			var caseCollection = reader.Read(stringReader);

			var runner = new TestSessionRunner(config, httpClient, GetRepository());

			// Act + Assert
			await runner.RunAsync(caseCollection);
		}
	}
}
