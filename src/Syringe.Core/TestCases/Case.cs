using System;
using System.Collections.Generic;
using System.Net;

namespace Syringe.Core.TestCases
{
	public class Case
	{
		public Guid Id { get; set; }
		public string Method { get; set; }
		public string Url { get; set; }
		public string PostBody { get; set; }
		public string ErrorMessage { get; set; }
		public string PostType { get; set; }
		public HttpStatusCode VerifyResponseCode { get; set; }
		public List<HeaderItem> Headers { get; set; }
		public string ParentFilename { get; set; }

		/// <summary>
		/// Number of seconds to sleep after the case runs
		/// </summary>
		public int Sleep { get; set; }

		public string ShortDescription { get; set; }
		public string LongDescription { get; set; }

		public List<ParseResponseItem> ParseResponses { get; set; }
		public List<VerificationItem> VerifyPositives { get; set; }
		public List<VerificationItem> VerifyNegatives { get; set; }
		public List<Variables> AvailableVariables { get; set; }

		public Case()
		{
			Headers = new List<HeaderItem>();
			ParseResponses = new List<ParseResponseItem>();
			VerifyPositives = new List<VerificationItem>();
			VerifyNegatives = new List<VerificationItem>();
            AvailableVariables = new List<Variables>();
		}

		public void AddHeader(string key, string value)
		{
			Headers.Add(new HeaderItem(key, value));
		}
	}
}