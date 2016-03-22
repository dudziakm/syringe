namespace Syringe.Core.Configuration
{
	public interface IHealthCheck
	{
		void CheckWebConfiguration();
		void CheckServiceConfiguration();
		void CheckServiceSwaggerIsRunning();
	}
}