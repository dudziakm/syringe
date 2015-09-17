using System;
using System.Threading;
using RestSharp;

namespace Syringe.Tests.Unit.StubsMocks
{
	public class RestClientMock : RestClient
	{
		public IRestResponse RestResponse { get; set; }

		public IRestRequest RestRequest { get; private set; }
		public TimeSpan ResponseTime { get; set; }

		public override IRestResponse Execute(IRestRequest request)
		{
			if (ResponseTime > TimeSpan.MinValue)
				Thread.Sleep(ResponseTime);

			RestRequest = request;
			return RestResponse;
		}
	}
}