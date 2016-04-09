using System.Collections.Generic;
using RestSharp;
using Syringe.Core.Services;
using Environment = Syringe.Core.Environment.Environment;

namespace Syringe.Client
{
	public class EnvironmentsClient : IEnvironmentsService
	{
		private readonly string _serviceUrl;
		private readonly RestSharpHelper _restSharpHelper;
		
		public EnvironmentsClient(string serviceUrl)
		{
			_serviceUrl = serviceUrl;
			_restSharpHelper = new RestSharpHelper("/api/environments");
		}

		public IEnumerable<Environment> List()
		{
			var client = new RestClient(_serviceUrl);
			IRestRequest request = _restSharpHelper.CreateRequest("List");

			IRestResponse response = client.Execute(request);
			return _restSharpHelper.DeserializeOrThrow<IEnumerable<Environment>>(response);
		}
	}
}