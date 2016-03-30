using System;
using System.Collections.Generic;
using System.Linq;
using Syringe.Core.Http;

namespace Syringe.Core.Tests.Results
{
	public class TestResult
	{
		public Guid Id { get; set; }
		public Guid SessionId { get; set; }
	    public Test TestTest { get; set; }
		public string ActualUrl { get; set; }
	    public string Message { get; set; }
	    public TimeSpan ResponseTime { get; set; }
        public List<Assertion> PositiveAssertionResults { get; set; }
		public List<Assertion> NegativeAssertionResults { get; set; }
		public bool ResponseCodeSuccess { get; set; }
		public HttpResponse HttpResponse { get; set; }
		public string ExceptionMessage { get; set; }

		public string HttpLog { get; set; }
		public string HttpContent { get; set; }
		public string Log { get; set; }

		public bool Success
		{
			get { return ResponseCodeSuccess && IsPositiveAssertionsSuccess && IsNegativeAssertionsSuccess; }
		}
		
		public bool IsPositiveAssertionsSuccess
		{
			get
			{
				if (PositiveAssertionResults == null || PositiveAssertionResults.Count == 0)
					return true;

				return PositiveAssertionResults.Count(x => x.Success == false) == 0;
			}
		}

		public bool IsNegativeAssertionsSuccess
		{
			get
			{
				if (NegativeAssertionResults == null || NegativeAssertionResults.Count == 0)
					return true;

				return NegativeAssertionResults.Count(x => x.Success == false) == 0;
			}
		}

		public TestResult()
		{
			PositiveAssertionResults = new List<Assertion>();
			NegativeAssertionResults = new List<Assertion>();
		}
	}
}
