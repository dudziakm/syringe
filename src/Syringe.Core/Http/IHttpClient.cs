using System.Collections.Generic;
using System.Threading.Tasks;
using Syringe.Core.TestCases;

namespace Syringe.Core.Http
{
	public interface IHttpClient
	{
		Task<HttpRequestInfo> ExecuteRequestAsync(string httpMethod, string url, string contentType, string postBody, IEnumerable<HeaderItem> headers);
	}
}