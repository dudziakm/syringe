using System;
using System.Collections.Generic;
using System.Net;
using Syringe.Core.Xml;

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
		public List<KeyValuePair<string, string>> Headers { get; set; }

		public string ShortDescription { get; set; }
		public string LongDescription { get; set; }

		public List<RegexItem> ParseResponses { get; set; }
		public List<RegexItem> VerifyPositives { get; set; }
		public List<RegexItem> VerifyNegatives { get; set; }

		public TestCase()
		{
			Headers = new List<KeyValuePair<string, string>>();
			ParseResponses = new List<RegexItem>();
			VerifyPositives = new List<RegexItem>();
			VerifyNegatives = new List<RegexItem>();
		}
	}
}