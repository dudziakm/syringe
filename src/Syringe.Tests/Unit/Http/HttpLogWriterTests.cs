using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
		public void WriteSeperator_should_write_seperator_to_textwriter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.WriteSeperator();

			// Assert
			Assert.That(stringBuilder.ToString(), Is.EqualTo(logWriter.Seperator + Environment.NewLine));
		}

		[Test]
		public void WriteRequest_should_ignore_null_method_parameter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.WriteRequest(null, "b", new List<KeyValuePair<string, string>>());

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void WriteRequest_should_ignore_null_url_parameter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.WriteRequest("http://www.google.com", null, new List<KeyValuePair<string, string>>());

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void WriteRequest_should_ignore_invalid_url_parameter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.WriteRequest("not a valid url", null, new List<KeyValuePair<string, string>>());

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void WriteRequest_should_allow_null_for_headers()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.WriteRequest("http://www.uri", "b", null);

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void WriteRequest_should_write_request_line_and_host_and_extra_newline_at_end()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.WriteRequest("post", "http://en.wikipedia.org/wiki/Microsoft?a=b", new List<KeyValuePair<string, string>>());

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.That(lines[0], Is.EqualTo("POST http://en.wikipedia.org/wiki/Microsoft?a=b HTTP/1.1"));
			Assert.That(lines[1], Is.EqualTo("Host: en.wikipedia.org"));
			Assert.That(lines[2], Is.EqualTo(""));
		}

		[Test]
		public void WriteRequest_should_append_headers_after_host()
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
			logWriter.WriteRequest("post", "http://en.wikipedia.org/wiki/Microsoft?a=b", headers);

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.That(lines[0], Is.EqualTo("POST http://en.wikipedia.org/wiki/Microsoft?a=b HTTP/1.1"));
			Assert.That(lines[1], Is.EqualTo("Host: en.wikipedia.org"));
			Assert.That(lines[2], Is.EqualTo("Cookie: aaa=bbb;loggedin=true"));
			Assert.That(lines[3], Is.EqualTo("Accept-Language: en-US"));
			Assert.That(lines[4], Is.EqualTo("Accept: text/html"));
			Assert.That(lines[5], Is.EqualTo(""));
		}

		[Test]
		public void WriteResponse_should_write_status_code_and_status_description_and_empty_line()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act	
			logWriter.WriteResponse(HttpStatusCode.NotFound, new List<KeyValuePair<string, string>>(), "");

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.That(lines[0], Is.EqualTo("HTTP/1.1 404 Not Found"));
			Assert.That(lines[1], Is.EqualTo(""));
		}

		[Test]
		public void WriteResponse_should_append_headers_and_response_body_and_empty_line()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);
			var headers = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("Server", "Apache"),
				new KeyValuePair<string, string>("Cache-Control", "private, s-maxage=0, max-age=0, must-revalidate"),
				new KeyValuePair<string, string>("Date", "Sun, 12 Apr 2015 19:18:21 GMT"),
				new KeyValuePair<string, string>("Content-Type", "text/html; charset=UTF-8")
			};

			// Act	
			logWriter.WriteResponse(HttpStatusCode.OK, headers, "<html><body></body></html>");

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.That(lines[0], Is.EqualTo("HTTP/1.1 200 OK"));
			Assert.That(lines[1], Is.EqualTo("Server: Apache"));
			Assert.That(lines[2], Is.EqualTo("Cache-Control: private, s-maxage=0, max-age=0, must-revalidate"));
			Assert.That(lines[3], Is.EqualTo("Date: Sun, 12 Apr 2015 19:18:21 GMT"));
			Assert.That(lines[4], Is.EqualTo("Content-Type: text/html; charset=UTF-8"));
			Assert.That(lines[5], Is.EqualTo(""));
			Assert.That(lines[6], Is.EqualTo("<html><body></body></html>"));
		}

		[Test]
		public void WriteResponse_should_ignore_null_headers_and_empty_body()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act	
			logWriter.WriteResponse(HttpStatusCode.OK, null, "");

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.That(lines[0], Is.EqualTo("HTTP/1.1 200 OK"));
			Assert.That(lines[1], Is.EqualTo(""));
		}
	}
}
