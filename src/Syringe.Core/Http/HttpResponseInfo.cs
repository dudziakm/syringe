using System;
using Newtonsoft.Json;
using RestSharp;

namespace Syringe.Core.Http
{
	public class HttpResponseInfo
	{
		public IRestResponse Response { get; set; }
		public TimeSpan ResponseTime { get; set; }
	}
}