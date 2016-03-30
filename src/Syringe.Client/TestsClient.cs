using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Client
{
	public class TestsClient : ITestService
	{
		private readonly string _serviceUrl;

		public TestsClient(string serviceUrl)
		{
			_serviceUrl = serviceUrl;
		}

		public IEnumerable<string> ListFilesForTeam(string branchName)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("ListForTeam");
			request.AddParameter("branchName", branchName);

			IRestResponse response = client.Execute(request);
			return DeserializeOrThrow<IEnumerable<string>>(response);
		}

		public Test GetTest(string filename, string branchName, Guid testId)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("GetTest");
			request.AddParameter("filename", filename);
			request.AddParameter("branchName", branchName);
			request.AddParameter("testId", testId);

			IRestResponse response = client.Execute(request);
			return DeserializeOrThrow<Test>(response);
		}

		public TestFile GetTestFile(string filename, string branchName)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("GetTestFile");
			request.AddParameter("filename", filename);
			request.AddParameter("branchName", branchName);

			IRestResponse response = client.Execute(request);

			return DeserializeOrThrow<TestFile>(response);
		}

	    public string GetXml(string filename, string branchName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("GetXml");
            request.AddParameter("filename", filename);
            request.AddParameter("branchName", branchName);

            IRestResponse response = client.Execute(request);

            return DeserializeOrThrow<string>(response);
        }

	    public bool EditTest(Test test, string branchName)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("EditTest");
			request.Method = Method.POST;
			request.AddJsonBody(test);
			request.AddQueryParameter("branchName", branchName);

			IRestResponse response = client.Execute(request);
			return DeserializeOrThrow<bool>(response);
		}

        public bool CreateTest(Test test, string branchName)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("CreateTest");
            request.Method = Method.POST;
            request.AddJsonBody(test);
            request.AddQueryParameter("branchName", branchName);

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public bool DeleteTest(Guid testId, string fileName, string branchName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("DeleteTest");
            request.Method = Method.POST;
            request.AddQueryParameter("testId", testId.ToString());
            request.AddQueryParameter("fileName", fileName);
            request.AddQueryParameter("branchName", branchName);

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public bool CreateTestFile(TestFile testFile, string branchName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("CreateTestFile");
            request.Method = Method.POST;
            request.AddJsonBody(testFile);
            request.AddQueryParameter("fileName", testFile.Filename);
            request.AddQueryParameter("branchName", branchName);

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public bool UpdateTestFile(TestFile testFile, string branchName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("UpdateTestFile");
            request.Method = Method.POST;
            request.AddJsonBody(testFile);
            request.AddQueryParameter("branchName", branchName);

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public IEnumerable<TestFileResultSummary> GetSummariesForToday()
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("GetSummariesForToday");
            request.Method = Method.GET;
            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<IEnumerable<TestFileResultSummary>>(response);
        }

	    public IEnumerable<TestFileResultSummary> GetSummaries()
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("GetSummaries");
            request.Method = Method.GET;
            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<IEnumerable<TestFileResultSummary>>(response);
        }

	    public TestFileResult GetResultById(Guid id)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("GetResultById");
            request.Method = Method.GET;
            request.AddQueryParameter("id", id.ToString());
            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<TestFileResult>(response);
        }

	    public Task DeleteResultAsync(Guid id)
	    {
            var client = new RestClient(_serviceUrl);

            IRestRequest request = CreateRequest("DeleteResultAsync");
            request.Method = Method.POST;
            request.AddQueryParameter("id", id.ToString());
            IRestResponse response = client.Execute(request);

            return DeserializeOrThrow<Task>(response);
        }

	    private T DeserializeOrThrow<T>(IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
			{
				return JsonConvert.DeserializeObject<T>(response.Content);
			}

            throw new Exception(response.Content);
        }

	    private IRestRequest CreateRequest(string action)
		{
			return new RestRequest(string.Format("/api/tests/{0}", action));
		}
	}
}