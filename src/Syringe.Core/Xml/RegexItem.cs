namespace Syringe.Core.Xml
{
	public class RegexItem
	{
		public string Description { get; set; }
		public string Regex { get; set; }

		public RegexItem(string description, string regex)
		{
			Description = description;
			Regex = regex;
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", Description, Regex);
		}
	}
}