using System.Collections.Generic;

namespace Syringe.Core.Xml
{
	public class TestCase
	{
		public int Id { get; set; }
		public List<string> Descriptions { get; internal set; }
		public string Method { get; set; }
		public string Url { get; set; }
		public string PostBody { get; set; }

		public TestCase()
		{
			Descriptions = new List<string>();
		}
	}
}