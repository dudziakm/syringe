using System;
using System.Collections.Generic;
using System.Net;
using Syringe.Core.Xml.LegacyConverter;

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

		public List<NumberedAttribute> Descriptions { get; internal set; }
        public List<NumberedAttribute> ParseResponses { get; set; }
        public List<NumberedAttribute> VerifyPositives { get; set; }
        public List<NumberedAttribute> VerifyNegatives { get; set; }

		public TestCase()
		{
            Descriptions = new List<NumberedAttribute>();
			AddHeader = new List<KeyValuePair<string, string>>();
		}
	}
}