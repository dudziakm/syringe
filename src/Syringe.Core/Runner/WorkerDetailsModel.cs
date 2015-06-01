using System.Collections.Generic;
using Syringe.Core.Results;

namespace Syringe.Core.Runner
{
	public class WorkerDetailsModel
	{
		public int TaskId { get; set; }
		public string Status { get; set; }
		public List<TestCaseResult> Results { get; set; }

		public int CurrentIndex { get; set; }
		public int TotalCases { get; set; }
		public string Errors { get; set; }

		public WorkerDetailsModel()
		{
			Results = new List<TestCaseResult>();
		}
	}
}