using System.Collections.Generic;
using System.Net;
using Syringe.Core;

namespace Syringe.Web.Models
{
    public class TestCaseViewModel
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public string PostBody { get; set; }
        public string ErrorMessage { get; set; }
        public PostType PostType { get; set; }
        public HttpStatusCode VerifyResponseCode { get; set; }
        public bool LogRequest { get; set; }
        public bool LogResponse { get; set; }
        public List<Header> Headers { get; set; }

        /// <summary>
        /// Number of seconds to sleep after the case runs
        /// </summary>
        public int Sleep { get; set; }

        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public List<ParsedResponseItem> ParseResponses { get; set; }

        public List<VerificationItem> Verifications { get; set; }

        public TestCaseViewModel()
        {
            Headers = new List<Header>();
            ParseResponses = new List<ParsedResponseItem>();
            Verifications = new List<VerificationItem>();
        }
    }
}