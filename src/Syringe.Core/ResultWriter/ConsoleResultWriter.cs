using System;
using System.Collections.Generic;
using Syringe.Core.Results;
using Syringe.Core.Xml;

namespace Syringe.Core.ResultWriter
{
	public class ConsoleResultWriter : IResultWriter
	{
		public void WriteHeader(string format, params object[] args)
		{
			
		}

		public void Write(TestCaseResult result)
		{
			WriteLine("Case {0} ({1})", result.TestCase.Id, result.TestCase.ShortDescription);
			WriteLine(" - Original url: {0}", result.TestCase.Url);
			WriteLine(" - Actual url : {0}", result.ActualUrl);
			WriteLine("{0}", (result.Success) ? "Passed" : "Failed");

			if (!string.IsNullOrEmpty(result.Message))
				WriteLine("{0}", result.Message);

			WriteLine("{0}", result.ResponseTime.ToString(@"mm\:ss\.FF"));

			WriteLine("Verify positives:");
			WriteVerifies(result.VerifyPositiveResults);

			WriteLine("Verify negatives:");
			WriteVerifies(result.VerifyNegativeResults);
		}

		protected virtual void WriteVerifies(List<RegexItem> verifyItems)
		{
			if (verifyItems.Count > 0)
			{
				foreach (RegexItem item in verifyItems)
				{
					WriteLine(" {0} - {1}", item.Description, item.Success);
				}
			}
			else
			{
				WriteLine("...none found");
			}
		}

		private void WriteLine(string format, params object[] args)
		{
			Console.WriteLine(format, args);
		}
	}
}