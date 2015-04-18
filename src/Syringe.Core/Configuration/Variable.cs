using System.Xml.Serialization;

namespace Syringe.Core.Configuration
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