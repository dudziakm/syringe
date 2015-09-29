namespace Syringe.Core.Configuration
{
	public interface IApplicationConfiguration
	{
		string TestCasesBaseDirectory { get; }
		string ServiceUrl { get; }
        string SignalRUrl { get; }
	}
}