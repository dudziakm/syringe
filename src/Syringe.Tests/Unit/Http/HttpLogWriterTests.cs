using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;
using Syringe.Core.Http.Logging;

namespace Syringe.Tests.Unit.Http
{
	public class HttpLogWriterTests
	{
		private HttpLogWriter GetHttpLogWriter(StringBuilder stringBuilder)
		{
			var logWriter = new HttpLogWriter(new StringWriter(stringBuilder));
			return logWriter;
		}

		[Test]
		public void WriteSeperator_should_write_seperator_to_textwriter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);

			// Act
			logWriter.AppendSeperator();

			// Assert
			Assert.That(stringBuilder.ToString(), Is.EqualTo(logWriter.Seperator + Environment.NewLine));
		}

		[Test]
		public void WriteRequest_should_ignore_null_method_parameter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);
			var requestDetails = new RequestDetails()
			{
				Method = null,
				Url = "url",
				Headers = new List<KeyValuePair<string, string>>()
			};

			// Act
			logWriter.AppendRequest(requestDetails);

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void WriteRequest_should_ignore_null_url_parameter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);
			var requestDetails = new RequestDetails()
			{
				Method = "get",
				Url = null,
				Headers = new List<KeyValuePair<string, string>>()
			};

			// Act
			logWriter.AppendRequest(requestDetails);

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void WriteRequest_should_ignore_invalid_url_parameter()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);
			var requestDetails = new RequestDetails()
			{
				Method = null,
				Url = "not a valid url",
				Headers = new List<KeyValuePair<string, string>>()
			};

			// Act
			logWriter.AppendRequest(requestDetails);

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void WriteRequest_should_allow_null_for_headers()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);
			var requestDetails = new RequestDetails()
			{
				Method = null,
				Url = "http://www.uri",
				Headers = null
			};

			// Act
			logWriter.AppendRequest(requestDetails);

			// Assert
			Assert.That(stringBuilder.ToString(), Is.Not.Null.Or.Empty);
		}

		[Test]
		public void WriteRequest_should_write_request_line_and_host_and_extra_newline_at_end()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var logWriter = GetHttpLogWriter(stringBuilder);
			var requestDetails = new RequestDetails()
			{
				Method = "post",
				Url = "http://en.wikipedia.org/wiki/Microsoft?a=b",
				Headers = new List<KeyValuePair<string, string>>()
			};

			// Act
			logWriter.AppendRequest(requestDetails);

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

			var requestDetails = new RequestDetails()
			{
				Method = "post",
				Url = "http://en.wikipedia.org/wiki/Microsoft?a=b",
				Headers = headers
			};

			// Act	
			logWriter.AppendRequest(requestDetails);

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
			var responseDetails = new ResponseDetails()
			{
				Status = HttpStatusCode.NotFound,
				BodyResponse = "",
				Headers = new List<KeyValuePair<string, string>>() 
			};

			// Act	
			logWriter.AppendResponse(responseDetails);

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

			var responseDetails = new ResponseDetails()
			{
				Status = HttpStatusCode.OK,
				BodyResponse = "<html><body></body></html>",
				Headers = headers
			};

			// Act	
			logWriter.AppendResponse(responseDetails);

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
			var responseDetails = new ResponseDetails()
			{
				Status = HttpStatusCode.OK,
				BodyResponse = "",
				Headers = null
			};

			// Act	
			logWriter.AppendResponse(responseDetails);

			// Assert
			string[] lines = stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.That(lines[0], Is.EqualTo("HTTP/1.1 200 OK"));
			Assert.That(lines[1], Is.EqualTo(""));
		}
	}
}
