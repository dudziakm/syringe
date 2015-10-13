using Quartz;
using Syringe.Service.Parallel;

namespace Syringe.Service.Schedule
{
	public class QuartzJob : IJob
	{
		private readonly ITestSessionQueue _caseQueue;

		public QuartzJob(ITestSessionQueue caseQueue)
	    {
		    _caseQueue = caseQueue;
	    }

		public void Execute(IJobExecutionContext context)
		{
			//_caseQueue.Add(context);
		}
	}
}
