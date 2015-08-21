using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Syringe.Core;

namespace Syringe.Web.Models
{
    public class TestCaseViewModel
    {
		[Required]
        public int Id { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }

        [Display(Name = "Post Body")]
        public string PostBody { get; set; }

		[Required]
        [Display(Name = "Error Message")]
        public string ErrorMessage { get; set; }

		[Required]
        [Display(Name = "Post Type")]
        public PostType PostType { get; set; }

        [Display(Name = "Verify Response Code")]
        public HttpStatusCode VerifyResponseCode { get; set; }

        [Display(Name = "Log Request")]
        public bool LogRequest { get; set; }

        [Display(Name = "Log Response")]
        public bool LogResponse { get; set; }

        public List<HeaderItem> Headers { get; set; }

        /// <summary>
        /// Number of seconds to sleep after the case runs
        /// </summary>
        public int Sleep { get; set; }

        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }

        [Display(Name = "Long Description")]
        public string LongDescription { get; set; }

        public List<ParseResponseItem> ParseResponses { get; set; }

        public List<VerificationItem> Verifications { get; set; }

        public string ParentFilename { get; set; }


        public TestCaseViewModel()
        {
            Headers = new List<HeaderItem>();
            ParseResponses = new List<ParseResponseItem>();
            Verifications = new List<VerificationItem>();
        }
    }
}