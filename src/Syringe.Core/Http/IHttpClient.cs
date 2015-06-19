using System.Collections.Generic;

namespace Syringe.Core.Http
{
	public interface IHttpClient
	{
		HttpResponse ExecuteRequest(string httpMethod, string url, string contentType, string postBody, IEnumerable<HeaderItem> headers);
		void LogLastRequest();
		void LogLastResponse();
	}
}