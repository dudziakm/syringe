using System;
using System.Collections.Generic;
using System.Net;

namespace Syringe.Core.Tests
{
	public class Test
	{
		public int Position { get; set; }
		public string ShortDescription { get; set; }
		public string LongDescription { get; set; }
		public string Method { get; set; }
		public string Url { get; set; }
		public string PostBody { get; set; }
		public string ErrorMessage { get; set; }
		public string PostType { get; set; }
		public HttpStatusCode VerifyResponseCode { get; set; }
		public List<HeaderItem> Headers { get; set; }

		public List<CapturedVariable> CapturedVariables { get; set; }
		public List<Assertion> VerifyPositives { get; set; }
		public List<Assertion> VerifyNegatives { get; set; }

		public string ParentFilename { get; set; }

		public Test()
		{
			Headers = new List<HeaderItem>();
			CapturedVariables = new List<CapturedVariable>();
			VerifyPositives = new List<Assertion>();
			VerifyNegatives = new List<Assertion>();
		}

		public void AddHeader(string key, string value)
		{
			Headers.Add(new HeaderItem(key, value));
		}
	}
}