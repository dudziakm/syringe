using System;
using System.Collections.Generic;

namespace Syringe.Core.Results
{
    public class TestCaseSession
    {
        public List<TestCaseResult> TestCaseResults { get; set; }

        public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

        public TimeSpan TotalRunTime { get; set; }
        public int TotalCasesRun { get; set; }
        public int TotalCasesPassed { get; set; }
        public int TotalCasesFailed { get; set; }
        public int TotalVerificationsPassed { get; set; }
        public int TotalVerificationsFailed { get; set; }
        public TimeSpan MaxResponseTime { get; set; }
        public TimeSpan MinResponseTime { get; set; }

        public TestCaseSession()
        {
            TestCaseResults = new List<TestCaseResult>();
        }
    }
}