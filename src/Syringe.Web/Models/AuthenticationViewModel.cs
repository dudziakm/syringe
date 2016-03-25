using System.ComponentModel.DataAnnotations;
using Syringe.Core.Configuration;
using Syringe.Core.TestCases;

namespace Syringe.Web.Models
{
    public class AuthenticationViewModel
    {
        public ApplicationConfig Configuration { get; set; }
        public string ReturnUrl { get; set; }
    }
}