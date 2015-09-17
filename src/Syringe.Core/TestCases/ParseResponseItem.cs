namespace Syringe.Core.TestCases
{
	public class ParseResponseItem
	{
		public string Description { get; set; }
		public string Regex { get; set; }

		public ParseResponseItem() : this("","")
		{
		}

		public ParseResponseItem(string description, string regex)
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