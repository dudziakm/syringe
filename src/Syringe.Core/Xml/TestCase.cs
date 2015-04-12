using System.Collections.Generic;

namespace Syringe.Core.Xml
{
	public class TestCase
	{
		public int Id { get; set; }
		public List<string> Descriptions { get; internal set; }

		public TestCase()
		{
			Descriptions = new List<string>();
		}
	}
}