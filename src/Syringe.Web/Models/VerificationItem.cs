using System.ComponentModel.DataAnnotations;
using Syringe.Core;

namespace Syringe.Web.Models
{
    public class VerificationItem
    {
        public string Description { get; set; }
        public string Regex { get; set; }

        [Display(Name = "Verify Type")]
        public string VerifyTypeValue { get; set; }

        [Display(Name = "Verify Type")]
        public VerifyType VerifyType { get; set; }
    }
}