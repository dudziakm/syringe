﻿using System;
using System.Configuration;

namespace Syringe.Core.Configuration
{
	public class ApplicationConfig : IApplicationConfiguration
	{
		private readonly Lazy<string> _lazySignalRUrl;

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

		public string WebsiteCorsUrl { get { return ConfigurationManager.AppSettings["WebsiteCorsUrl"]; } }

		public string TestCasesBaseDirectory
		{
			get { return ConfigurationManager.AppSettings["TestCasesBaseDirectory"]; }
		}

		public string ServiceUrl
		{
			get { return ConfigurationManager.AppSettings["ServiceUrl"]; }
		}

		public string SignalRUrl
		{
			get
			{
				return _lazySignalRUrl.Value;
			}
		}
	}
}