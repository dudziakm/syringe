using System.Collections.Generic;

namespace Syringe.Core.Xml
{
	public class TestCaseContainer
	{
		public int Repeat { get; set; }
		public IEnumerable<TestCase> TestCases { get; internal set; }

		// Custom variables, e.g. <testvar varname="LOGIN_URL">http://myserver/login.php</testvar>
		public Dictionary<string, string> Variables { get; internal set; }

		public TestCaseContainer()
		{
			Variables = new Dictionary<string, string>();
			TestCases = new List<TestCase>();
		}
	}
}