namespace Syringe.Core.Tests
{
	public class Variable
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public Environment.Environment Environment { get; set; }

		public Variable() { }

		public Variable(string name, string value, string environment)
		{
			Name = name;
			Value = value;
			Environment = new Environment.Environment() { Name = environment };
		}
	}
}