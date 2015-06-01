using System.Collections.Generic;
using System.Web.Helpers;
using RestSharp;
using Syringe.Core;

namespace Syringe.Web.ApiClient
{
	public class CasesClient
	{
		private readonly string _baseUrl;

		public CasesClient()
		{
			_baseUrl = "http://localhost:1232";
		}

		public CaseCollection GetByFilename(string filename, string teamName)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("GetByFilename");
			request.AddParameter("filename", filename);
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			CaseCollection collection = Json.Decode<CaseCollection>(response.Content);

			return collection;
		}

		public IEnumerable<string> ListForTeam(string teamName)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("ListForTeam");
			request.AddParameter("teamName", teamName);

			IRestResponse response = client.Execute(request);
			IEnumerable<string> collection = Json.Decode<IEnumerable<string>>(response.Content);

			return collection;
		}

		private IRestRequest CreateRequest(string action)
		{
			return new RestRequest(string.Format("/api/cases/{0}", action));
		}
	}
}