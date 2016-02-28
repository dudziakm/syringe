using System.ComponentModel.DataAnnotations;
using Syringe.Core.TestCases;

namespace Syringe.Web.Models
{
    public class VerificationItemModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string Regex { get; set; }

        [Display(Name = "Verify Type")]
        public VerifyType VerifyType { get; set; }
    }
}