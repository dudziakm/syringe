using System;
using System.Threading;
using RestSharp;
using Syringe.Core.Domain.Entities.Teamcity;
using Syringe.Core.Domain.Providers;

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