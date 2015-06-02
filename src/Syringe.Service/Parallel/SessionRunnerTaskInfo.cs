using System;
using System.Threading;
using System.Threading.Tasks;
using Syringe.Core.Domain.Entities;
using Syringe.Core.Runner;

namespace Syringe.Service.Parallel
{
	internal class SessionRunnerTaskInfo
	{
		public string Username { get; set; }
		public string TeamName { get; set; }
		public DateTime StartTime { get; set; }
		public TaskRequest Request { get; set; }
		public TestSessionRunner Runner { get; set; }

		public CancellationTokenSource CancelTokenSource { get; set; }
		public Task CurrentTask { get; set; }

		public string Errors { get; set; }
	}
}