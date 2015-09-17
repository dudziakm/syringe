using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Services;
using Syringe.Core.Tasks;

namespace Syringe.Client
{
	public class TasksClient : ITasksService
	{
		// Don't use the Restsharp JSON deserializer, it fails
		private readonly string _baseUrl;

		public TasksClient()
			: this(new ApplicationConfig())
		{
		}

		public TasksClient(IApplicationConfiguration appConfig)
		{
			_baseUrl = appConfig.ServiceUrl;
		}
		public int Start(TaskRequest item)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("start");
			request.AddJsonBody(item);
			request.Method = Method.POST;

			IRestResponse response = client.Execute(request);
			return ParseOrDefault(response.Content, 0);
		}

		public string Stop(int id)
		{
			throw new NotImplementedException();
		}

		public List<string> StopAll()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TaskDetails> GetRunningTasks()
		{
			throw new NotImplementedException();
		}

		public TaskDetails GetRunningTaskDetails(int taskId)
		{
			var client = new RestClient(_baseUrl);
			IRestRequest request = CreateRequest("GetRunningTaskDetails");
			request.AddParameter("taskId", taskId);

			IRestResponse response = client.Execute(request);
			TaskDetails details = JsonConvert.DeserializeObject<TaskDetails>(response.Content);

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