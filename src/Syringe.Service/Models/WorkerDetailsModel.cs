using System;
using Syringe.Core;

namespace Syringe.Service.Models
{
	public class WorkerDetailsModel
	{
		public int TaskId { get; set; }
		public string Status { get; set; }
		public Case CurrentTestCase { get; set; }

		public int Count { get; set; }
		public int TotalCases { get; set; }
		public string Errors { get; set; }
	}
}