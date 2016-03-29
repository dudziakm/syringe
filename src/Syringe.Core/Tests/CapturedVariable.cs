namespace Syringe.Core.Tests
{
	public class CapturedVariable
	{
		public string Name { get; set; }
		public string Regex { get; set; }

		public CapturedVariable() : this("","")
		{
		}

		public CapturedVariable(string name, string regex)
		{
			Name = name;
			Regex = regex;
		}
	}
}