using System.Collections.Generic;
using Syringe.Core.Xml;

namespace Syringe.Core
{
	public class TestCaseCollection
	{
		public int Repeat { get; set; }
		public IEnumerable<TestCase> TestCases { get; set; }

		// Custom variables, e.g. <testvar varname="LOGIN_URL">http://myserver/login.php</testvar>
		public Dictionary<string, string> Variables { get; set; }

		public TestCaseCollection()
		{
			Variables = new Dictionary<string, string>();
			TestCases = new List<TestCase>();
		}
	}
}