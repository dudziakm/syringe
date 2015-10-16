using System;
using System.Linq;
using System.Collections.Generic;

namespace Syringe.Core.Results
{
    public class TestCaseSession
    {
	    public Guid Id { get; set; }
	    public string TestCaseFilename { get; set; }

        public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
        public TimeSpan TotalRunTime { get; set; }
        public int TotalCasesRun { get; set; }
		public TimeSpan MaxResponseTime { get; set; }
		public TimeSpan MinResponseTime { get; set; }
		public List<TestCaseResult> TestCaseResults { get; set; }

		public int TotalCasesPassed
	    {
		    get { return TestCaseResults.Count(x => x.Success == true); }
	    }

		public int TotalCasesFailed
		{
			get { return TestCaseResults.Count(x => x.Success == false); }
		}

		public int TotalVerificationsPassed
		{
			get
			{
				return TestCaseResults.Sum(x => x.VerifyPositiveResults.Count(v => v.Success == true) +
												x.VerifyNegativeResults.Count(v => v.Success == true));
			}
		}

		public int TotalVerificationsFailed
		{
			get 
			{ 
				return TestCaseResults.Sum(x => x.VerifyPositiveResults.Count(v => v.Success == false) +
												x.VerifyNegativeResults.Count(v => v.Success == false)); 
			}
		}

        public TestCaseSession()
        {
            TestCaseResults = new List<TestCaseResult>();
        }
    }
}