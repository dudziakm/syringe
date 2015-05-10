using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Tests.Unit.StubsMocks;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Tests.Unit.Http
{
	public class HttpClientTests
	{
		private HttpLogWriterStub _loggerStub;
		private RestClientMock _restClientMock;

		[Test]
		public void should_throw_exception_when_uri_is_invalid()
		{
			// Arrange
			var restResponse = new RestResponse();
			HttpClient httpClient = CreateClient(restResponse);

			string url = "invalid url";

			string method = "get";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<KeyValuePair<string, string>>();

			// Act + Assert
			Assert.Throws<ArgumentException>(() => httpClient.ExecuteRequest(method, url, contentType, postBody, headers));
		}

		[Test]
		public void should_return_expected_html_content()
		{
			// Arrange
			var restResponse = new RestResponse()
			{
				RawBytes = Encoding.UTF8.GetBytes("<html>some text </html>")
			};
			HttpClient httpClient = CreateClient(restResponse);

			string expectedContent = _restClientMock.HttpResponse.Content;

			string method = "get";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<KeyValuePair<string, string>>();

			// Act
			HttpResponse response = httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Assert.That(response, Is.Not.Null);
			Assert.That(response.Content, Is.EqualTo(expectedContent));
		}

		[Test]
		public void should_ignore_null_headers()
		{
			// Arrange
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "get";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<KeyValuePair<string, string>>();

			// Act
			HttpResponse response = httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Assert.That(response, Is.Not.Null);
		}

		[Test]
		public void should_add_postbody_and_contenttype_to_request_when_method_is_post()
		{
			// Arrange
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "post";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "keywords=foo&location=london";
			var headers = new List<KeyValuePair<string, string>>();

			// Act
			httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Parameter parameter = _restClientMock.RestRequest.Parameters.First();
			Assert.That(parameter.Name, Is.EqualTo(contentType));
			Assert.That(parameter.Value, Is.EqualTo(postBody));
			Assert.That(parameter.Type, Is.EqualTo(ParameterType.RequestBody));
		}

		[Test]
		public void should_use_httpmethod_get_when_method_is_invalid()
		{
			// Arrange
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "snort";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<KeyValuePair<string, string>>();

			// Act
			httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Assert.That(_restClientMock.RestRequest.Method, Is.EqualTo(Method.GET));
		}

		[Test]
		public void should_add_headers()
		{
			// Arrange
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "get";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("user-agent", "Netscape Navigator 1"),
				new KeyValuePair<string, string>("cookies", "mmm cookies"),
			};

			// Act
			httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			var parameters = _restClientMock.RestRequest.Parameters;
			var userAgent = parameters.First(x => x.Name == "user-agent");
			var cookies = parameters.First(x => x.Name == "cookies");

			Assert.That(parameters.Count, Is.EqualTo(2));
			Assert.That(userAgent.Value, Is.EqualTo("Netscape Navigator 1"));
			Assert.That(cookies.Value, Is.EqualTo("mmm cookies"));
		}

		// TODO:
		// - response properties
		// - lastrequest
		// - lastresponse
		// - response times
		// - logging
		// - header conversion from the response

		private HttpClient CreateClient(RestResponse restResponse)
		{
			_loggerStub = new HttpLogWriterStub();
			_restClientMock = new RestClientMock();
			_restClientMock.HttpResponse = restResponse;
			return new HttpClient(_loggerStub, _restClientMock);
		}
	}
}