﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp;
using Syringe.Core.Http;
using Syringe.Core.TestCases;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Unit.Http
{
	public class HttpClientTests
	{
		private RestClientMock _restClientMock;

		[Test]
		public async Task should_throw_exception_when_uri_is_invalid()
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

			try
			{
				await httpClient.ExecuteRequestAsync(method, url, contentType, postBody, headers);
				Assert.Fail("Should have thrown an exception.");
			}
			catch (ArgumentException)
			{
			}
		}

		[Test]
		public async Task should_return_expected_html_content()
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
			HttpResponseInfo responseInfo = await httpClient.ExecuteRequestAsync(method, url, contentType, postBody, headers);

			// Assert
			Assert.NotNull(responseInfo);
			Assert.AreEqual(restResponse.Content, responseInfo.Response.Content);
		}

		[Test]
		public async Task should_ignore_null_headers()
		{
			// Arrange
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "get";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<HeaderItem>();

			// Act
			HttpResponseInfo responseInfo = await httpClient.ExecuteRequestAsync(method, url, contentType, postBody, headers);

			// Assert
			Assert.IsNotNull(responseInfo);
		}

		[Test]
		public async Task should_add_postbody_and_contenttype_to_request_when_method_is_post()
		{
			// Arrange
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "post";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "keywords=foo&location=london";
			var headers = new List<HeaderItem>();

			// Act
			await httpClient.ExecuteRequestAsync(method, url, contentType, postBody, headers);

			// Assert
			Parameter parameter = _restClientMock.RestRequest.Parameters.First();
			Assert.AreEqual(contentType, parameter.Name);
			Assert.AreEqual(postBody, parameter.Value);
			Assert.AreEqual(ParameterType.RequestBody, parameter.Type);
		}

		[Test]
		public async Task should_use_httpmethod_get_when_method_is_invalid()
		{
			// Arrange
			HttpClient httpClient = CreateClient(new RestResponse());

			string method = "snort";
			string url = "http://www.example.com";
			string contentType = "text/html";
			string postBody = "";
			var headers = new List<HeaderItem>();

			// Act
			await httpClient.ExecuteRequestAsync(method, url, contentType, postBody, headers);

			// Assert
			Assert.AreEqual(Method.GET, _restClientMock.RestRequest.Method);
		}

		[Test]
		public async Task should_add_headers()
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
			await httpClient.ExecuteRequestAsync(method, url, contentType, postBody, headers);

			// Assert
			var parameters = _restClientMock.RestRequest.Parameters;
			var userAgent = parameters.First(x => x.Name == "user-agent");
			var cookies = parameters.First(x => x.Name == "cookies");

			Assert.AreEqual(2, parameters.Count);
			Assert.AreEqual("Netscape Navigator 1", userAgent.Value);
			Assert.AreEqual("mmm cookies", cookies.Value);
		}

		[Test]
		public async Task should_fill_response_properties()
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
			HttpResponseInfo responseInfo = await httpClient.ExecuteRequestAsync(method, url, contentType, postBody, headers);

			// Assert
			Assert.AreEqual(restResponseStub.StatusCode, responseInfo.Response.StatusCode);
			Assert.AreEqual(restResponseStub.Content, responseInfo.Response.Content);
			Assert.AreEqual(restResponseStub.Headers.Count, responseInfo.Response.Headers.Count);
		}

		[Test]
		public async Task should_record_response_times()
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
			HttpResponseInfo responseInfo = await httpClient.ExecuteRequestAsync(method, url, contentType, postBody, headers);

			// Assert
			Assert.That(responseInfo.ResponseTime, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)));
		}

		private HttpClient CreateClient(IRestResponse restResponse)
		{
			_restClientMock = new RestClientMock();
			_restClientMock.RestResponse = restResponse;
			return new HttpClient(_restClientMock);
		}
	}
}