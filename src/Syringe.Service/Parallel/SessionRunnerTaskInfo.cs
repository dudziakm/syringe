using System;
using System.Threading;
using System.Threading.Tasks;
using Syringe.Core.Runner;
using Syringe.Service.Models;

namespace Syringe.Service.Parallel
{
	internal class SessionRunnerTaskInfo
	{
		public DateTime StartTime { get; set; }
		public RunCaseCollectionRequestModel Request { get; set; }
		public TestSessionRunner Runner { get; set; }

		public CancellationTokenSource CancelTokenSource { get; set; }
		public Task CurrentTask { get; set; }

		public string Errors { get; set; }
	}
}