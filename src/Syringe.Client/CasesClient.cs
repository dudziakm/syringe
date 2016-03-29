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
	public class CasesClient : ICaseService
	{
		private readonly string _serviceUrl;

		public CasesClient(string serviceUrl)
		{
			_serviceUrl = serviceUrl;
		}

		public IEnumerable<string> ListFilesForTeam(string teamName)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("ListForTeam");
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			return DeserializeOrThrow<IEnumerable<string>>(response);
		}

		public Test GetTestCase(string filename, string teamName, Guid caseId)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("GetTest");
			request.AddParameter("filename", filename);
			request.AddParameter("caseId", caseId);
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			return DeserializeOrThrow<Test>(response);
		}

		public TestFile GetTestCaseCollection(string filename, string teamName)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("GetTestFile");
			request.AddParameter("filename", filename);
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);

			return DeserializeOrThrow<TestFile>(response);
		}

	    public string GetXmlTestCaseCollection(string filename, string teamName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("GetXml");
            request.AddParameter("filename", filename);
            request.AddParameter("teamName", teamName);

            IRestResponse response = client.Execute(request);

            return DeserializeOrThrow<string>(response);
        }

	    public bool EditTestCase(Test testTest, string teamName)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("EditTestCase");
			request.Method = Method.POST;
			request.AddJsonBody(testTest);
			request.AddQueryParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			return DeserializeOrThrow<bool>(response);
		}

        public bool CreateTestCase(Test testTest, string teamName)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("CreateTest");
            request.Method = Method.POST;
            request.AddJsonBody(testTest);
            request.AddQueryParameter("teamName", teamName);

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public bool DeleteTestCase(Guid testCaseId, string fileName, string teamName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("DeleteTest");
            request.Method = Method.POST;
            request.AddQueryParameter("fileName", fileName);
            request.AddQueryParameter("teamName", teamName);
            request.AddQueryParameter("testCaseId", testCaseId.ToString());

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public bool CreateTestFile(TestFile testFile, string teamName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("CreateTestFile");
            request.Method = Method.POST;
            request.AddJsonBody(testFile);
            request.AddQueryParameter("fileName", testFile.Filename);
            request.AddQueryParameter("teamName", teamName);

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public bool UpdateTestFile(TestFile testFile, string teamName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("UpdateTestFile");
            request.Method = Method.POST;
            request.AddJsonBody(testFile);
            request.AddQueryParameter("teamName", teamName);

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

	    public TestFileResult GetById(Guid caseId)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("GetById");
            request.Method = Method.GET;
            request.AddQueryParameter("caseId", caseId.ToString());
            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<TestFileResult>(response);
        }

	    public Task DeleteAsync(Guid sessionId)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("DeleteAsync");
            request.Method = Method.POST;
            request.AddQueryParameter("sessionId", sessionId.ToString());
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
			return new RestRequest(string.Format("/api/cases/{0}", action));
		}
	}
}