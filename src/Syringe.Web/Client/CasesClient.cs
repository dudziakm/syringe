using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using RestSharp;
using Syringe.Core;
using Syringe.Web.Controllers;

namespace Syringe.Web.Client
{
	public class CasesClient
	{
		private readonly string _baseUrl;

		public CasesClient()
		{
			_baseUrl = "http://localhost:1232";
		}

		public CaseCollection GetCaseCollection(string filename)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("GetByFilename");
			request.AddParameter("filename", filename);

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