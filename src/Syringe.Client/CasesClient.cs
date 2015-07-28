using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Syringe.Core;
using Syringe.Core.Configuration;
using Syringe.Core.Domain.Services;

namespace Syringe.Client
{
	public class CasesClient : ICaseService
	{
		private readonly string _baseUrl;

		public CasesClient()
			: this(new ApplicationConfig())
		{
		}

		public CasesClient(IApplicationConfiguration appConfig)
		{
			_baseUrl = appConfig.ServiceUrl;
		}

		public IEnumerable<string> ListFilesForTeam(string teamName)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("ListForTeam");
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			IEnumerable<string> collection = JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content);

			return collection;
		}

		public Case GetTestCase(string filename, string teamName, int caseId)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("GetTestCase");
			request.AddParameter("filename", filename);
			request.AddParameter("caseId", caseId);
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			Case collection = JsonConvert.DeserializeObject<Case>(response.Content);

			return collection;
		}

		public CaseCollection GetTestCaseCollection(string filename, string teamName)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("GetTestCaseCollection");
			request.AddParameter("filename", filename);
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			CaseCollection collection = JsonConvert.DeserializeObject<CaseCollection>(response.Content);

			return collection;
		}

		public bool AddTestCase(Case testCase, string teamName)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("AddTestCase");
			request.Method = Method.POST;
			request.AddJsonBody(testCase);
			request.AddQueryParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			return JsonConvert.DeserializeObject<Boolean>(response.Content);
		}

		private IRestRequest CreateRequest(string action)
		{
			return new RestRequest(string.Format("/api/cases/{0}", action));
		}
	}
}