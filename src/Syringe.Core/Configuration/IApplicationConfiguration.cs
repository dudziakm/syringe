namespace Syringe.Core.Configuration
{
	public interface IApplicationConfiguration
	{
		string WebsiteUrl { get; }
		string TestCasesBaseDirectory { get; }
		string ServiceUrl { get; }
		string SignalRUrl { get; }
	}
}