namespace Syringe.Core.Tests
{
	public class Assertion
	{
		public string Description { get; set; }
		public string Regex { get; set; }
		public string TransformedRegex { get; set; }
		public bool Success { get; set; }
		public AssertionType AssertionType { get; set; }
		public string Log { get; set; }

		public Assertion() : this("","", AssertionType.Positive)
		{
		}

		public Assertion(string description, string regex, AssertionType assertionType)
		{
			Description = description;
			Regex = regex;
			AssertionType = assertionType;
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", Description, TransformedRegex);
		}
	}
}