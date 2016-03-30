using Syringe.Core.Tests;

namespace Syringe.Core.Xml.Writer
{
	public interface ITestFileWriter
	{
		string Write(TestFile testFile);
	}
}