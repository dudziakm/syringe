using Syringe.Core.Configuration;

namespace Syringe.Web.Models
{
    public class AuthenticationViewModel
    {
        public IConfiguration Configuration { get; set; }
        public string ReturnUrl { get; set; }
    }
}