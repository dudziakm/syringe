using System.Collections.Generic;
using Syringe.Core.Results;

namespace Syringe.Core.Domain.Entities
{
	public class TaskDetails
	{
		public int TaskId { get; set; }
		public string Status { get; set; }
		public List<TestCaseResult> Results { get; set; }

		public int CurrentIndex { get; set; }
		public int TotalCases { get; set; }
		public string Errors { get; set; }

		public TaskDetails()
		{
			Results = new List<TestCaseResult>();
		}
	}
}