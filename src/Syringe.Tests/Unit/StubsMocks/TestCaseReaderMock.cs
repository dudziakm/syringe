using Syringe.Core;
using Syringe.Core.Xml;

namespace Syringe.Tests.Unit.StubsMocks
{
	public class TestCaseReaderMock : ITestCaseReader
	{
		public CaseCollection CaseCollection { get; set; }

		public CaseCollection Read()
		{
			return CaseCollection;
		}
	}
}