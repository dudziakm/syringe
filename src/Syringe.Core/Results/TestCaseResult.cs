using System;
using System.Collections.Generic;
using System.Linq;
using Syringe.Core.Http;
using Syringe.Core.TestCases;

namespace Syringe.Core.Results
{
	public class TestCaseResult
	{
		public Guid Id { get; set; }
		public Guid SessionId { get; set; }
	    public Case TestCase { get; set; }
		public string ActualUrl { get; set; }
	    public string Message { get; set; }
	    public TimeSpan ResponseTime { get; set; }
        public List<VerificationItem> VerifyPositiveResults { get; set; }
		public List<VerificationItem> VerifyNegativeResults { get; set; }
		public bool ResponseCodeSuccess { get; set; }
		public HttpResponse HttpResponse { get; set; }
		public string ExceptionMessage { get; set; }

		public string HttpLog { get; set; }
		public string HttpContent { get; set; }
		public string Log { get; set; }

		public bool Success
		{
			get { return ResponseCodeSuccess && VerifyPositivesSuccess && VerifyNegativeSuccess; }
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
			Id = Guid.NewGuid();
		}
	}
}
