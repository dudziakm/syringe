using System.IO;

namespace Syringe.Core.Xml.Reader
{
	public interface ITestCaseReader
    {
		CaseCollection Read(TextReader textReader);
    }
}