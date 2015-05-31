using Syringe.Core;
using Syringe.Core.Results;

namespace Syringe.Web.Client
{
	public class WorkerDetails
	{
		public int TaskId { get; set; }
		public string Status { get; set; }
		public Case CurrentTestCase { get; set; }
		public TestCaseResult LastResult { get; set; }

		public int Count { get; set; }
		public int TotalCases { get; set; }
		public string Errors { get; set; }
	}
}