namespace Syringe.Core
{
	public class RegexItem
	{
		public string Description { get; set; }
		public string Regex { get; set; }
		public bool Success { get; set; }

		public RegexItem() : this("","")
		{
		}

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