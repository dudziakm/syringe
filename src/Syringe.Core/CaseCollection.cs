using System.Collections.Generic;

namespace Syringe.Core
{
	public class CaseCollection
	{
		public int Repeat { get; set; }
		public IEnumerable<Case> TestCases { get; set; }

		// Custom variables, e.g. <testvar varname="LOGIN_URL">http://myserver/login.php</testvar>
		public Dictionary<string, string> Variables { get; set; }

		public CaseCollection()
		{
			Variables = new Dictionary<string, string>();
			TestCases = new List<Case>();
		}
	}
}