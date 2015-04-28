using System.Collections.Generic;

namespace Syringe.Core.Results.Writer
{
	public abstract class ResultWriterBase : IResultWriter
	{
		public void WriteHeader(string format, params object[] args)
		{
			WriteLine(new string('-', 80));
			WriteLine(format, args);
		}

		public void Write(TestCaseResult result)
		{
			// Header
			WriteHeader("Case {0} ({1})", result.TestCase.Id, result.TestCase.ShortDescription);
			WriteLine("");

			// Result info
			WriteLine(" - Original url: {0}", result.TestCase.Url);
			WriteLine(" - Actual url: {0}", result.ActualUrl);
			WriteLine(" - Response code success: {0}", (result.VerifyResponseCodeSuccess) ? "Passed" : "Failed");
			WriteLine(" - Time taken: {0}", result.ResponseTime.ToString());
			WriteLine(" - Success: {0}", (result.Success) ? "Passed" : "Failed");
			if (!string.IsNullOrEmpty(result.Message))
				WriteLine(" - Message: {0}", result.Message);

			// Positives
			WriteLine("Verify positives");
			WriteLine(" - Success: {0}", (result.VerifyPositivesSuccess) ? "Passed" : "Failed");
			WriteVerifies(result.VerifyPositiveResults);

			// Negatives
			WriteLine("Verify negatives");
			WriteLine(" - Success: {0}", (result.VerifyNegativeSuccess) ? "Passed" : "Failed");
			WriteVerifies(result.VerifyNegativeResults);
		}

		protected virtual void WriteVerifies(List<RegexItem> verifyItems)
		{
			if (verifyItems.Count > 0)
			{
				foreach (RegexItem item in verifyItems)
				{
					WriteLine("  - {0} - {1}", item.Description, item.Success);
				}
			}
			else
			{
				WriteLine("  - (none found)");
			}
		}

		protected abstract void WriteLine(string format, params object[] args);
	}
}