using System.IO;
using Syringe.Core;
using Syringe.Core.TestCases;
using Syringe.Core.Xml;
using Syringe.Core.Xml.Reader;

namespace Syringe.Tests.Unit.StubsMocks
{
	public class TestCaseReaderMock : ITestCaseReader
	{
		public CaseCollection CaseCollection { get; set; }

		public CaseCollection Read(TextReader textReader)
		{
			return CaseCollection;
		}
	}
}