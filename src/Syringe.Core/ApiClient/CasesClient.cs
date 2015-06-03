using System.Collections.Generic;
using System.Web.Helpers;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Domain.Service;

namespace Syringe.Core.ApiClient
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
			IEnumerable<string> collection = Json.Decode<IEnumerable<string>>(response.Content);

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
			Case collection = Json.Decode<Case>(response.Content);

			return collection;
		}

		public CaseCollection GetTestCaseCollection(string filename, string teamName)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("GetTestCaseCollection");
			request.AddParameter("filename", filename);
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			CaseCollection collection = Json.Decode<CaseCollection>(response.Content);

			return collection;
		}

		private IRestRequest CreateRequest(string action)
		{
			return new RestRequest(string.Format("/api/cases/{0}", action));
		}
	}
}