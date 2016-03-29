using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using Syringe.Core.Results;
using Syringe.Core.Services;
using Syringe.Core.TestCases;

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

		public Case GetTestCase(string filename, string teamName, int index)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("GetTestCase");
			request.AddParameter("filename", filename);
			request.AddParameter("index", index);
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			return DeserializeOrThrow<Case>(response);
		}

		public CaseCollection GetTestCaseCollection(string filename, string teamName)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("GetTestCaseCollection");
			request.AddParameter("filename", filename);
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);

			return DeserializeOrThrow<CaseCollection>(response);
		}

	    public string GetXmlTestCaseCollection(string filename, string teamName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("GetXmlTestCaseCollection");
            request.AddParameter("filename", filename);
            request.AddParameter("teamName", teamName);

            IRestResponse response = client.Execute(request);

            return DeserializeOrThrow<string>(response);
        }

	    public bool EditTestCase(Case testCase, string teamName)
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = CreateRequest("EditTestCase");
			request.Method = Method.POST;
			request.AddJsonBody(testCase);
			request.AddQueryParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			return DeserializeOrThrow<bool>(response);
		}

        public bool CreateTestCase(Case testCase, string teamName)
        {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("CreateTestCase");
            request.Method = Method.POST;
            request.AddJsonBody(testCase);
            request.AddQueryParameter("teamName", teamName);

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public bool DeleteTestCase(int position, string fileName, string teamName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("DeleteTestCase");
            request.Method = Method.POST;
            request.AddQueryParameter("fileName", fileName);
            request.AddQueryParameter("teamName", teamName);
            request.AddQueryParameter("position", position.ToString());

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public bool CreateTestFile(CaseCollection caseCollection, string teamName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("CreateTestFile");
            request.Method = Method.POST;
            request.AddJsonBody(caseCollection);
            request.AddQueryParameter("fileName", caseCollection.Filename);
            request.AddQueryParameter("teamName", teamName);

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public bool UpdateTestFile(CaseCollection caseCollection, string teamName)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("UpdateTestFile");
            request.Method = Method.POST;
            request.AddJsonBody(caseCollection);
            request.AddQueryParameter("teamName", teamName);

            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<bool>(response);
        }

	    public IEnumerable<SessionInfo> GetSummariesForToday()
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("GetSummariesForToday");
            request.Method = Method.GET;
            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<IEnumerable<SessionInfo>>(response);
        }

	    public IEnumerable<SessionInfo> GetSummaries()
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("GetSummaries");
            request.Method = Method.GET;
            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<IEnumerable<SessionInfo>>(response);
        }

	    public TestCaseSession GetById(Guid caseId)
	    {
            var client = new RestClient(_serviceUrl);
            IRestRequest request = CreateRequest("GetById");
            request.Method = Method.GET;
            request.AddQueryParameter("caseId", caseId.ToString());
            IRestResponse response = client.Execute(request);
            return DeserializeOrThrow<TestCaseSession>(response);
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