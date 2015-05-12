using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
		private HttpLogWriterMock _httpLogWriterMock;
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
			var restResponse = new RestResponseStub()
			{
				Content = "<html>some text </html>"
			};
			HttpClient httpClient = CreateClient(restResponse);


			string method = "get";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<KeyValuePair<string, string>>();

			// Act
			HttpResponse response = httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Assert.That(response, Is.Not.Null);
			Assert.That(response.Content, Is.EqualTo(restResponse.Content));
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

		[Test]
		public void should_fill_response_properties()
		{
			// Arrange
			var restResponseStub = new RestResponseStub();
			restResponseStub.Content = "HTTP/1.1 200 OK\nServer: Apache\n\n<html>some text </html>";
			restResponseStub.StatusCode = HttpStatusCode.Accepted;
			restResponseStub.Headers = new Parameter[] { new Parameter(), new Parameter()};

			HttpClient httpClient = CreateClient(restResponseStub);

			string method = "get";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<KeyValuePair<string, string>>();

			// Act
			HttpResponse response = httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(restResponseStub.StatusCode));
			Assert.That(response.Content, Is.EqualTo(restResponseStub.Content));
			Assert.That(response.Headers.Count, Is.EqualTo(restResponseStub.Headers.Count));
		}


		[Test]
		public void should_convert_headers()
		{
			// Arrange

			// Act

			// Assert
		}

		[Test]
		public void should_record_last_request()
		{
			// Arrange
			HttpClient httpClient = CreateClient(new RestResponse());

			string contentType = "text/html";
			var request1Headers = new List<KeyValuePair<string, string>>();
			var request2Headers = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("user-agent", "Frozen Olaf Browser 4"),
				new KeyValuePair<string, string>("cookies", "mmm cookies"),
			};

			httpClient.ExecuteRequest("get", "http://www.example1.com", contentType, "request 1", request1Headers);
			httpClient.ExecuteRequest("put", "http://www.example2.com", contentType, "request 2", request2Headers);

			// Act
			httpClient.LogLastRequest();

			// Assert
			Assert.That(_httpLogWriterMock.RequestDetails.Url, Is.EqualTo("http://www.example2.com"));
			Assert.That(_httpLogWriterMock.RequestDetails.Method, Is.EqualTo("put"));
			Assert.That(_httpLogWriterMock.RequestDetails.Body, Is.EqualTo("request 2"));
			Assert.That(_httpLogWriterMock.RequestDetails.Headers.Count(), Is.EqualTo(2));
		}

		[Test]
		public void should_record_last_response()
		{
			// Arrange

			// Act

			// Assert
		}

		[Test]
		public void should_record_response_times()
		{
			// Arrange
			HttpClient httpClient = CreateClient(new RestResponse());
			_restClientMock.ResponseTime = TimeSpan.FromSeconds(1);

			string method = "get";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<KeyValuePair<string, string>>();

			// Act
			HttpResponse response = httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Assert.That(response.ResponseTime, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)));
		}

		private HttpClient CreateClient(IRestResponse restResponse)
		{
			_httpLogWriterMock = new HttpLogWriterMock();
			_restClientMock = new RestClientMock();
			_restClientMock.RestResponse = restResponse;
			return new HttpClient(_httpLogWriterMock, _restClientMock);
		}
	}
}