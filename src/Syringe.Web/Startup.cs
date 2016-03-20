using System;
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
using Syringe.Core.Configuration;
using Syringe.Core.Exceptions;

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
			// OAuth2 Integrations
			//
			var config = new ApplicationConfig();
			ThrowIfInvalidOAuthConfig(config);

			// Console: https://console.developers.google.com/home/dashboard
			// Found under API and credentials.
			app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
			{
				ClientId = config.GoogleAuthClientId,
				ClientSecret = config.GoogleAuthClientSecret
			});

			// Console: https://account.live.com/developers/applications/
			// Make sure he 'redirecturl' is set to 'http://localhost:1980/Authentication/Noop' (or the domain being used), to match the CallbackPath
			app.UseMicrosoftAccountAuthentication(new MicrosoftAccountAuthenticationOptions()
			{
				ClientId = config.MicrosoftAuthClientId,
				ClientSecret = config.MicrosoftAuthClientSecret,
				CallbackPath = new PathString("/Authentication/Noop")
			});

			// Console:  https://github.com/settings/developers
			// Set the callback url in the Github console to the same as the homepage url.
			app.UseGitHubAuthentication(new GitHubAuthenticationOptions()
			{
				ClientId = config.GithubAuthClientId,
				ClientSecret = config.GithubAuthClientSecret
			});
		}

		private void ThrowIfInvalidOAuthConfig(ApplicationConfig config)
		{
			if (string.IsNullOrEmpty(config.GoogleAuthClientId) || string.IsNullOrEmpty(config.GoogleAuthClientSecret))
			{
				throw new ConfigurationException("Please enter a valid Google client id/client secret in the web.config");
			}

			if (string.IsNullOrEmpty(config.MicrosoftAuthClientId) || string.IsNullOrEmpty(config.MicrosoftAuthClientSecret))
			{
				throw new ConfigurationException("Please enter a valid Microsoft client id/client secret in the web.config");
			}

			if (string.IsNullOrEmpty(config.GithubAuthClientId) || string.IsNullOrEmpty(config.GithubAuthClientSecret))
			{
				throw new ConfigurationException("Please enter a valid Github client id/client secret in the web.config");
			}
		}
	}
}
