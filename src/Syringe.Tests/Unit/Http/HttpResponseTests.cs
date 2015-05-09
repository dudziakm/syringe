using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;
using Syringe.Core.Http;

namespace Syringe.Tests.Unit.Http
{
	public class HttpResponseTests
	{
		[Test]
		public void should_create_headers_in_ctor()
		{
			// Arrange
			var response = new HttpResponse();

			// Act + Assert
			Assert.That(response.Headers, Is.Not.Null);
			Assert.That(response.ResponseTime, Is.Not.Null);
		}

		[Test]
		public void ToString_should_append_headers_and_response_body_and_empty_line()
		{
			// Arrange
			var response = new HttpResponse();
			response.Headers = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("Server", "Apache"),
				new KeyValuePair<string, string>("Cache-Control", "private, s-maxage=0, max-age=0, must-revalidate"),
				new KeyValuePair<string, string>("Date", "Sun, 12 Apr 2015 19:18:21 GMT"),
				new KeyValuePair<string, string>("Content-Type", "text/html; charset=UTF-8")
			};

			response.Content = "<html><body></body></html>";
			response.StatusCode = HttpStatusCode.OK;

			// Act	
			string content = response.ToString();

			// Assert
			string[] lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			Assert.That(lines[0], Is.EqualTo("HTTP/1.1 200 OK"));
			Assert.That(lines[1], Is.EqualTo("Server: Apache"));
			Assert.That(lines[2], Is.EqualTo("Cache-Control: private, s-maxage=0, max-age=0, must-revalidate"));
			Assert.That(lines[3], Is.EqualTo("Date: Sun, 12 Apr 2015 19:18:21 GMT"));
			Assert.That(lines[4], Is.EqualTo("Content-Type: text/html; charset=UTF-8"));
			Assert.That(lines[5], Is.EqualTo(""));
			Assert.That(lines[6], Is.EqualTo("<html><body></body></html>"));
		}
	}
}