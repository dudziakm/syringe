using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using RestSharp;
using Syringe.Core;
using Syringe.Core.Http;
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
			var headers = new List<HeaderItem>();

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
			var headers = new List<HeaderItem>();

			// Act
			HttpResponse response = httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Assert.NotNull(response);
			Assert.AreEqual(restResponse.Content, response.Content);
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
			var headers = new List<HeaderItem>();

			// Act
			HttpResponse response = httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Assert.IsNotNull(response);
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
			var headers = new List<HeaderItem>();

			// Act
			httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Parameter parameter = _restClientMock.RestRequest.Parameters.First();
			Assert.AreEqual(contentType, parameter.Name);
			Assert.AreEqual(postBody, parameter.Value);
			Assert.AreEqual(ParameterType.RequestBody, parameter.Type);
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
			var headers = new List<HeaderItem>();

			// Act
			httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Assert.AreEqual(Method.GET, _restClientMock.RestRequest.Method);
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
			var headers = new List<HeaderItem>()
			{
				new HeaderItem("user-agent", "Netscape Navigator 1"),
				new HeaderItem("cookies", "mmm cookies"),
			};

			// Act
			httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			var parameters = _restClientMock.RestRequest.Parameters;
			var userAgent = parameters.First(x => x.Name == "user-agent");
			var cookies = parameters.First(x => x.Name == "cookies");

			Assert.AreEqual(2, parameters.Count);
			Assert.AreEqual("Netscape Navigator 1", userAgent.Value);
			Assert.AreEqual("mmm cookies", cookies.Value);
		}

		[Test]
		public void should_fill_response_properties()
		{
			// Arrange
			var restResponseStub = new RestResponseStub();
			restResponseStub.Content = "HTTP/1.1 200 OK\nServer: Apache\n\n<html>some text </html>";
			restResponseStub.StatusCode = HttpStatusCode.Accepted;
			restResponseStub.Headers = new Parameter[] { new Parameter(), new Parameter() };

			HttpClient httpClient = CreateClient(restResponseStub);

			string method = "get";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<HeaderItem>();

			// Act
			HttpResponse response = httpClient.ExecuteRequest(method, url, contentType, postBody, headers);

			// Assert
			Assert.AreEqual(restResponseStub.StatusCode, response.StatusCode);
			Assert.AreEqual(restResponseStub.Content, response.Content);
			Assert.AreEqual(restResponseStub.Headers.Count, response.Headers.Count);
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
			var request1Headers = new List<HeaderItem>();
			var request2Headers = new List<HeaderItem>()
			{
				new HeaderItem("user-agent", "Frozen Olaf Browser 4"),
				new HeaderItem("cookies", "mmm cookies"),
			};

			httpClient.ExecuteRequest("get", "http://www.example1.com", contentType, "request 1", request1Headers);
			httpClient.ExecuteRequest("put", "http://www.example2.com", contentType, "request 2", request2Headers);

			// Act
			httpClient.LogLastRequest();

			// Assert
			Assert.AreEqual("http://www.example2.com", _httpLogWriterMock.RequestDetails.Url);
			Assert.AreEqual("put", _httpLogWriterMock.RequestDetails.Method);
			Assert.AreEqual("request 2", _httpLogWriterMock.RequestDetails.Body);
			Assert.AreEqual(2, _httpLogWriterMock.RequestDetails.Headers.Count());
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
			var headers = new List<HeaderItem>();

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