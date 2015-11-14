using System;

namespace Syringe.Core.Results
{
	public class SessionInfo
	{
		public Guid Id { get; set; }
        public DateTime DateRun { get; set; }
        public string TestCaseFileName { get; set; }
	    public TimeSpan TotalRunTime { get; set; }
	    public int TotalPassed { get; set; }
	    public int TotalFailed { get; set; }
	    public int TotalRun { get; set; }
	}
}