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
        [Required]
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

        [Required]
        public int Sleep { get; set; }

        [Display(Name = "Short Description")]
        [Required]
        public string ShortDescription { get; set; }

        [Display(Name = "Long Description")]
        [Required]
        public string LongDescription { get; set; }
        public List<ParseResponseItem> ParseResponses { get; set; }
        public List<VerificationItemModel> Verifications { get; set; }
        [Required]
        public string ParentFilename { get; set; }
        public TestCaseViewModel()
        {
            Headers = new List<HeaderItem>();
            ParseResponses = new List<ParseResponseItem>();
            Verifications = new List<VerificationItemModel>();
        }
    }
}