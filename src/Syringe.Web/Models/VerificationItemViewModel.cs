using System.ComponentModel.DataAnnotations;

namespace Syringe.Web.Models
{
    public class VerificationItemViewModel
    {
        public string Description { get; set; }
        public string Regex { get; set; }

        [Display(Name = "Verify Type")]
        public string VerifyType { get; set; }
    }
}