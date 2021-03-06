﻿namespace Syringe.Core.Configuration
{
	public interface IConfiguration
	{
		string ServiceUrl { get; set; }
		string WebsiteUrl { get; set; }
		string TestFilesBaseDirectory { get; set; }
		OAuthConfiguration OAuthConfiguration { get; set; }
		GitConfiguration GitConfiguration { get; set; }
	}
}