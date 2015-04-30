using System;
using System.Collections.Generic;
using Syringe.Core.Xml;
using System.Linq;

namespace Syringe.Core.Results
{
	public class TestCaseResult
	{
	    public Case TestCase { get; set; }
		public string ActualUrl { get; set; }
	    public string Message { get; set; }
	    public TimeSpan ResponseTime { get; set; }
        public List<VerificationItem> VerifyPositiveResults { get; set; }
		public List<VerificationItem> VerifyNegativeResults { get; set; }
		public bool VerifyResponseCodeSuccess { get; set; }

		public bool Success
		{
			get { return VerifyResponseCodeSuccess && VerifyPositivesSuccess && VerifyNegativeSuccess; }
		}
		
		public bool VerifyPositivesSuccess
		{
			get
			{
				if (VerifyPositiveResults == null || VerifyPositiveResults.Count == 0)
					return true;

				return VerifyPositiveResults.Count(x => x.Success == false) == 0;
			}
		}

		public bool VerifyNegativeSuccess
		{
			get
			{
				if (VerifyNegativeResults == null || VerifyNegativeResults.Count == 0)
					return true;

				return VerifyNegativeResults.Count(x => x.Success == false) == 0;
			}
		}

		public TestCaseResult()
		{
			VerifyPositiveResults = new List<VerificationItem>();
			VerifyNegativeResults = new List<VerificationItem>();
		}
	}
}
