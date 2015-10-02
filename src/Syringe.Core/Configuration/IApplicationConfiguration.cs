namespace Syringe.Core.Configuration
{
	public interface IApplicationConfiguration
	{
		string WebsiteCorsUrl { get; }
		string TestCasesBaseDirectory { get; }
		string ServiceUrl { get; }
		string SignalRUrl { get; }
	}
}