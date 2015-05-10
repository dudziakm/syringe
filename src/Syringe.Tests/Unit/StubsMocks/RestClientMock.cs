using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Deserializers;

namespace Syringe.Tests.Unit.StubsMocks
{
	public class RestClientMock : RestClient
	{
		public RestResponse HttpResponse { get; set; }
		public IRestRequest RestRequest { get; private set; }

		public override IRestResponse Execute(IRestRequest request)
		{
			RestRequest = request;
			return HttpResponse;
		}
	}
}