using System;
using System.Configuration;

namespace Syringe.Core.Configuration
{
	public class ApplicationConfig : IApplicationConfiguration
	{
		private readonly Lazy<string> _lazySignalRUrl;

		public string WebsiteUrl => ConfigurationManager.AppSettings["WebsiteUrl"];
		public string TestCasesBaseDirectory => ConfigurationManager.AppSettings["TestCasesBaseDirectory"];
		public string ServiceUrl => ConfigurationManager.AppSettings["ServiceUrl"];
		public string SignalRUrl => _lazySignalRUrl.Value;

		public string GithubAuthClientId => ConfigurationManager.AppSettings["GithubAuthClientId"];
		public string GithubAuthClientSecret => ConfigurationManager.AppSettings["GithubAuthClientSecret"];

		public string GoogleAuthClientId => ConfigurationManager.AppSettings["GoogleAuthClientId"];
		public string GoogleAuthClientSecret => ConfigurationManager.AppSettings["GoogleAuthClientSecret"];

		public string MicrosoftAuthClientId => ConfigurationManager.AppSettings["MicrosoftAuthClientId"];
		public string MicrosoftAuthClientSecret => ConfigurationManager.AppSettings["MicrosoftAuthClientSecret"];

		public ApplicationConfig()
		{
			_lazySignalRUrl = new Lazy<string>(BuildSignalRUrl);
		}

		private string BuildSignalRUrl()
		{
			var builder = new UriBuilder(ServiceUrl);
			builder.Path = "signalr";
			return builder.ToString();
		}
	}
}