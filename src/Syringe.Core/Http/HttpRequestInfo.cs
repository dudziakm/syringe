using System;
using RestSharp;

namespace Syringe.Core.Http
{
	public class HttpRequestInfo
	{
		public IRestRequest Request { get; set; }
		public IRestResponse Response { get; set; }
		public TimeSpan ResponseTime { get; set; }
	}
}