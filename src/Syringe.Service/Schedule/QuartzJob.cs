using Quartz;
using Syringe.Service.Parallel;

namespace Syringe.Service.Schedule
{
	public class QuartzJob : IJob
	{
		private readonly ParallelTestSessionQueue _caseQueue;

		public QuartzJob()
	    {
		    _caseQueue = ParallelTestSessionQueue.Default;
	    }

		public void Execute(IJobExecutionContext context)
		{
			//_caseQueue.Add(context);
		}
	}
}
