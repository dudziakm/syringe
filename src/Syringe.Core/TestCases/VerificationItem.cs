namespace Syringe.Core.TestCases
{
	public class VerificationItem
	{
		public string Description { get; set; }
		public string Regex { get; set; }
		public string TransformedRegex { get; set; }
		public bool Success { get; set; }
		public VerifyType VerifyType { get; set; }

		public VerificationItem() : this("","", VerifyType.Positive)
		{
		}

		public VerificationItem(string description, string regex, VerifyType verifyType)
		{
			Description = description;
			Regex = regex;
			VerifyType = verifyType;
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", Description, TransformedRegex);
		}
	}
}