namespace Syringe.Core.TestCases
{
	public class Variable
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public Environment.Environment Environment { get; set; }

		public Variable() { }

		public Variable(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}