using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.MicrosoftAccount;
using Owin;
using Syringe.Web;
using Owin.Security.Providers.GitHub;

[assembly: OwinStartup(typeof(Startup))]

namespace Syringe.Web
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			ConfigureOAuth(app);
		}

		private void ConfigureOAuth(IAppBuilder app)
		{
			var cookieOptions = new CookieAuthenticationOptions
			{
				LoginPath = new PathString("/Authentication/Login"),
				CookieName = "SyringeOAuth"
			};

			app.UseCookieAuthentication(cookieOptions);
			app.SetDefaultSignInAsAuthenticationType(cookieOptions.AuthenticationType);

			//
			// Put these client id/secrets into the web.config or app.config
			//

			app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
			{
				ClientId = "1055607189262-k919rn22c7ddgf6j20n8m96qqve9kl1h.apps.googleusercontent.com",
				ClientSecret = "8-yD74T-fO-feDrXceg3ll_i"
			});

			// Make sure https://account.live.com/developers/applications
			// has the 'redirecturl' set to 'http://localhost:1980/Authentication/Noop' (or the domain being used), to match the CallbackPath
			app.UseMicrosoftAccountAuthentication(new MicrosoftAccountAuthenticationOptions()
			{
				ClientId = "000000004416FD0A",
				ClientSecret = "ssba5FOQCsg5E18baVQbjoe075fEJd2x",
				CallbackPath = new PathString("/Authentication/Noop")
			});

			// For Github: https://github.com/settings/applications/
			// Set the callback url in the Github console to the same as the homepage url.
			app.UseGitHubAuthentication(new GitHubAuthenticationOptions()
			{
				ClientId = "783a88d1d712e79804b4",
				ClientSecret = "76391542e4d77126aa5136ed2a6e74f2f85dd46f"
			});

			// TODO: logout drop down needs to switch to Login when not logged in
		}
	}
}
