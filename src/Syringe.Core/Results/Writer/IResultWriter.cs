namespace Syringe.Core.Results.Writer
{
	public interface IResultWriter
	{
		void WriteHeader(string format, params object[] args);
		void Write(TestCaseResult result);
	}
}
