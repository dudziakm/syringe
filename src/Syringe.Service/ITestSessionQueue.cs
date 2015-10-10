using System.Collections.Generic;
using Syringe.Core.Tasks;

namespace Syringe.Service
{
    internal interface ITestSessionQueue
    {
        /// <summary>
        /// Adds a request to run a test case XML file the queue of tasks to run.
        /// </summary>
        int Add(TaskRequest item);

        /// <summary>
        /// Shows minimal information about all test case XML file requests in the queue, and their status,
        /// and who started the run.
        /// </summary>
        IEnumerable<TaskDetails> GetRunningTasks();

        /// <summary>
        /// Shows the full information about a *single* test case run - it doesn't have to be running, it could be complete.
        /// This includes the results of every case in the collection of cases for the XML file run.
        /// </summary>
        TaskDetails GetRunningTaskDetails(int taskId);

        /// <summary>
        /// Stops a case XML request task in the queue, returning a message of whether the stop succeeded or not.
        /// </summary>
        string Stop(int id);

        /// <summary>
        /// Attempts to shut down all running tasks.
        /// </summary>
        List<string> StopAll();
    }
}