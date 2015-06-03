namespace Syringe.Core.Configuration
{
	public interface IApplicationConfiguration
	{
		string TestCasesBaseDirectory { get; }
		string ServiceUrl { get; }
	}
}