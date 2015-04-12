using System;
using System.Collections.Generic;
using System.Net;

namespace Syringe.Core
{
	public class TestCase
	{
		public int Id { get; set; }
		public string Method { get; set; }
		public string Url { get; set; }
		public string PostBody { get; set; }
		public string ErrorMessage { get; set; }
		public string PostType { get; set; }
		public HttpStatusCode VerifyResponseCode { get; set; }
		public bool LogRequest { get; set; }
		public bool LogResponse { get; set; }
		public int Sleep { get; set; }
		public List<KeyValuePair<string, string>> AddHeader { get; set; }

		public List<string> Descriptions { get; internal set; }
		public List<string> ParseResponses { get; set; }
		public List<string> VerifyPositives { get; set; }
		public List<string> VerifyNegatives { get; set; }

		public TestCase()
		{
			Descriptions = new List<string>();
			AddHeader = new List<KeyValuePair<string, string>>();
		}
	}
}