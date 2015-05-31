using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using RestSharp;
using Syringe.Core.Runner;
using Syringe.Web.Controllers;

namespace Syringe.Web.Client
{
	public class TasksClient
	{
		private readonly string _baseUrl;

		public TasksClient()
		{
			_baseUrl = "http://localhost:1232";
		}

		public int Start(string filename, string username)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("start");
			request.AddJsonBody(new
			{
				Filename = filename,
				Username = username
			});
			request.Method = Method.POST;

			IRestResponse response = client.Execute(request);
			return ParseOrDefault(response.Content, 0);
		}

		public WorkerDetailsModel GetProgress(int taskId)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("GetRunningTaskDetails");
			request.AddParameter("taskId", taskId);

			// Don't use the Restsharp JSON deserializer, it fails
			IRestResponse response = client.Execute(request);
			WorkerDetailsModel details = Json.Decode<WorkerDetailsModel>(response.Content);

			return details;
		}

		private IRestRequest CreateRequest(string action)
		{
			return new RestRequest(string.Format("/api/tasks/{0}", action));
		}

		public static int ParseOrDefault(string value, int defaultValue)
		{
			int result;
			if (int.TryParse(value, out result))
				return result;

			return defaultValue;
		}
	}
}