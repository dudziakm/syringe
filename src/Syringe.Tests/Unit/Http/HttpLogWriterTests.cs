using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Core.Http;
using Syringe.Tests.Unit.StubsMocks;

namespace Syringe.Tests.Unit.Http
{
	public class HttpLogWriterTests
	{
		private HttpLogWriter GetHttpLogWriter(StringBuilder stringBuilder)
		{
			var factory = new TextWriterFactoryMock(stringBuilder);
			var logWriter = new HttpLogWriter(factory);
			return logWriter;
		}

		[Test]
		public void LogRequest_should_ignore_null_method_parameter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.LogRequest(null, "b", new List<KeyValuePair<string, string>>());

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void LogRequest_should_ignore_null_url_parameter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.LogRequest("http://www.google.com", null, new List<KeyValuePair<string, string>>());

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void LogRequest_should_ignore_invalid_url_parameter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.LogRequest("not a valid url", null, new List<KeyValuePair<string, string>>());

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void LogRequest_should_allow_null_for_headers()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.LogRequest("http://www.uri", "b", null);

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void LogRequest_should_write_request_line_and_host_and_extra_newline_at_end()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.LogRequest("post", "http://en.wikipedia.org/wiki/Microsoft?a=b", new List<KeyValuePair<string, string>>());

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.That(lines[0], Is.EqualTo("POST http://en.wikipedia.org/wiki/Microsoft?a=b HTTP/1.1"));
			Assert.That(lines[1], Is.EqualTo("Host: en.wikipedia.org"));
			Assert.That(lines[2], Is.EqualTo(""));
		}

		[Test]
		public void LogRequest_should_append_headers_after_host()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);
			var headers = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("Cookie", "aaa=bbb;loggedin=true"),
				new KeyValuePair<string, string>("Accept-Language", "en-US"),
				new KeyValuePair<string, string>("Accept", "text/html")
			};

			// Act	
			logWriter.LogRequest("post", "http://en.wikipedia.org/wiki/Microsoft?a=b", headers);

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.That(lines[0], Is.EqualTo("POST http://en.wikipedia.org/wiki/Microsoft?a=b HTTP/1.1"));
			Assert.That(lines[1], Is.EqualTo("Host: en.wikipedia.org"));
			Assert.That(lines[2], Is.EqualTo("Cookie: aaa=bbb;loggedin=true"));
			Assert.That(lines[3], Is.EqualTo("Accept-Language: en-US"));
			Assert.That(lines[4], Is.EqualTo("Accept: text/html"));
			Assert.That(lines[5], Is.EqualTo(""));
		}
	}
}
