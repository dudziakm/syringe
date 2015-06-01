using System.Configuration;

namespace Syringe.Service.Configuration
{
	public class Config : IConfiguration
	{
		public string TestCasesBaseDirectory
		{
			get { return ConfigurationManager.AppSettings["TestCasesBaseDirectory"]; }
		}
	}
}