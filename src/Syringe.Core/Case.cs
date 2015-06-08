using System;
using System.Collections.Generic;
using System.Net;
using Syringe.Core.Xml;

namespace Syringe.Core
{
	public class Case
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
		public List<KeyValuePair<string, string>> Headers { get; set; }

		/// <summary>
		/// Number of seconds to sleep after the case runs
		/// </summary>
		public int Sleep { get; set; }

		public string ShortDescription { get; set; }
		public string LongDescription { get; set; }

		public List<ParseResponseItem> ParseResponses { get; set; }
		public List<VerificationItem> VerifyPositives { get; set; }
		public List<VerificationItem> VerifyNegatives { get; set; }

		public Case()
		{
			Headers = new List<KeyValuePair<string, string>>();
			ParseResponses = new List<ParseResponseItem>();
			VerifyPositives = new List<VerificationItem>();
			VerifyNegatives = new List<VerificationItem>();
		}

		public void AddHeader(string key, string value)
		{
			Headers.Add(new KeyValuePair<string, string>(key, value));
		}
	}
}