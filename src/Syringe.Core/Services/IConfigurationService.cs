using Syringe.Core.Configuration;

namespace Syringe.Client
{
	public interface IConfigurationService
	{
		IConfiguration GetConfiguration();
	}
}