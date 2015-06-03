using System.Configuration;

namespace Syringe.Core.Configuration
{
	public class ApplicationConfig : IApplicationConfiguration
	{
		public string TestCasesBaseDirectory
		{
			get { return ConfigurationManager.AppSettings["TestCasesBaseDirectory"]; }
		}

		public string ServiceUrl
		{
			get { return ConfigurationManager.AppSettings["ServiceUrl"]; }
		}
	}
}