using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Syringe.Core.Canary;
using Syringe.Core.Extensions;
using Syringe.Core.Repositories;
using Syringe.Core.Results;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
    public class AuthenticationController : Controller
    {
		public ActionResult Login(string returnUrl)
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Authentication", new { ReturnUrl = returnUrl }));
		}

		public ActionResult Logout()
	    {
			FormsAuthentication.SignOut();
			Response.Cookies["SyringeOAuth"].Expires = DateTime.Now.AddDays(-1);
			Response.Cookies[".AspNet.Cookies"].Expires = DateTime.Now.AddDays(-1);

		    return Redirect("/");
	    }

		public ActionResult Noop()
		{
			// Just for you Microsoft
			return Redirect("/");
		}

		public ActionResult ExternalLoginCallback(string returnUrl)
		{
			// TODO: make these into a configuration object, for example Github uses "{urn:github:name: Chris S.}" for its name
			var claims = System.Security.Claims.ClaimsPrincipal.Current.Claims.ToList();
			var nameIdentifier = claims.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", StringComparison.InvariantCultureIgnoreCase));
			var uidIdentifier = claims.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", StringComparison.InvariantCultureIgnoreCase));

			if (nameIdentifier == null || uidIdentifier == null)
			{
				string debugInfo = "";
				foreach (Claim claim in claims)
				{
					debugInfo += $"{claim.Type} : {claim.Value}\n";
				}

				throw new InvalidOperationException("The OAuth provider didn't provide a name or nameidentifier:\n " + debugInfo);
			}

			string id = uidIdentifier.Value;
			string name = nameIdentifier.Value;

			string userData = JsonConvert.SerializeObject(new UserContext() {FullName = name, Id = id});
			string encryptedData = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(1, "Syringe", DateTime.Now, DateTime.UtcNow.AddDays(1), true, userData));

			// Add UserData to the forms auth cookie by setting the cookie manually.
			Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedData)
			{
				Expires = DateTime.Now.AddDays(1)
			});

			//return Content($"Redirect url: {returnUrl}, uid: {id}, name: {name}");
			return Redirect(returnUrl);
		}

		// Implementation copied from a standard MVC Project, with some stuff
		// that relates to linking a new external login to an existing identity
		// account removed.
		private class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
	}
}