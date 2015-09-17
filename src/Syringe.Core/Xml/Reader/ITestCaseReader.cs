using System.IO;
using Syringe.Core.TestCases;

namespace Syringe.Core.Xml.Reader
{
	public interface ITestCaseReader
    {
		CaseCollection Read(TextReader textReader);
    }
}