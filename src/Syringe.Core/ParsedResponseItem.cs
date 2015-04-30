namespace Syringe.Core
{
	public class ParsedResponseItem
	{
		public string Description { get; set; }
		public string Regex { get; set; }

		public ParsedResponseItem() : this("","")
		{
		}

		public ParsedResponseItem(string description, string regex)
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