using System.Collections.Generic;

namespace Syringe.Core.TestCases
{
	public class CaseCollection
	{
		public int Repeat { get; set; }
		public IEnumerable<Case> TestCases { get; set; }
		public string Filename { get; set; }
		public List<Variable> Variables { get; set; }

		public CaseCollection()
		{
			Variables = new List<Variable>();
			TestCases = new List<Case>();
		}
	}
}