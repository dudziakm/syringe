using System.Collections.Generic;
using Syringe.Core.Results;

namespace Syringe.Core.Http
{
	public interface IHttpClient
	{
		HttpResponse ExecuteRequest(string httpMethod, string url, string contentType, string postBody, IEnumerable<KeyValuePair<string, string>> headers);
		void LogLastRequest();
		void LogLastResponse();
	}
}