using System;
using System.Collections.Generic;
using Syringe.Core.Xml;

namespace Syringe.Core.Results
{
	public class TestCaseResult
	{
	    public Case TestCase { get; set; }
		public string ActualUrl { get; set; }
	    public bool Success { get; set; }
	    public string Message { get; set; }
	    public TimeSpan ResponseTime { get; set; }

        public List<RegexItem> VerifyPositiveResults { get; set; }
		public List<RegexItem> VerifyNegativeResults { get; set; }
	}
}
