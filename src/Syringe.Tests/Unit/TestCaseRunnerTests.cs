using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Results;
using Syringe.Core.Runner;
using Syringe.Core.Xml;
using Syringe.Tests.Unit.StubsMocks;

namespace Syringe.Tests.Unit
{
	public class TestCaseRunnerTests
	{
		private string ReadEmbeddedFile(string file)
		{
			string path = string.Format("Syringe.Tests.Unit.{0}", file);

			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
			if (stream == null)
				throw new InvalidOperationException(string.Format("Unable to find '{0}' as an embedded resource", path));

			string result = "";
			using (StreamReader reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}

			return result;
		}

		[Test]
		public void should_do_something()
		{
			// Arrange
			var mockHttpClient = new HttpClientMock();
			mockHttpClient.Response = GetHttpResponse();
			var runner = new TestSessionRunner(new Config(), mockHttpClient, new HttpLogWriterStub());
			string xml = ReadEmbeddedFile("basic.xml");

			// Act
			TestCaseSession summary = runner.Run(new TestCaseReader(), new StringReader(xml));

			// Assert
		}

		private HttpResponse GetHttpResponse()
		{
			var response = new HttpResponse();
			response.StatusCode = HttpStatusCode.OK;
			response.Content = HttpTestData.Basic();

			return response;
		}
	}
}
