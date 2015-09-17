namespace Syringe.Core.TestCases.Configuration
{
	public class Variable
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public Variable(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}