using System.Collections.Generic;
using System.Web.Helpers;
using RestSharp;
using Syringe.Core.Domain.Entities;
using Syringe.Core.Domain.Service;
using Syringe.Core.Runner;

namespace Syringe.Core.ApiClient
{
	public class TasksClient : ITasksService
	{
		// Don't use the Restsharp JSON deserializer, it fails
		private readonly string _baseUrl;

		public TasksClient()
			: this("http://localhost:1232")
		{
		}

		public TasksClient(string url)
		{
			_baseUrl = url;
		}

		public int Start(TaskRequest item)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("start");

			IRestResponse response = client.Execute(request);
			return ParseOrDefault(response.Content, 0);
		}

		public string Stop(int id)
		{
			throw new System.NotImplementedException();
		}

		public List<string> StopAll()
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<TaskDetails> GetRunningTasks()
		{
			throw new System.NotImplementedException();
		}

		public TaskDetails GetRunningTaskDetails(int taskId)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("GetRunningTaskDetails");
			request.AddParameter("taskId", taskId);

			IRestResponse response = client.Execute(request);
			TaskDetails details = Json.Decode<TaskDetails>(response.Content);

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