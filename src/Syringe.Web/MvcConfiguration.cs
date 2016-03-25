using System;
using System.Configuration;

namespace Syringe.Web
{
	public class MvcConfiguration
	{
		private readonly Lazy<string> _lazySignalRUrl;
		public string SignalRUrl => _lazySignalRUrl.Value;

		public string ServiceUrl { get; set; }

		public MvcConfiguration()
		{
			ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"];
			if (string.IsNullOrEmpty(ServiceUrl))
			{
				ServiceUrl = "http://localhost:1981";
			}

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