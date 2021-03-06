using System.Collections.Generic;
using Syringe.Core.Tasks;

namespace Syringe.Service
{
    public interface ITestFileQueue
    {
        /// <summary>
        /// Adds a request to run a test XML file the queue of tasks to run.
        /// </summary>
        int Add(TaskRequest item);

        /// <summary>
        /// Shows minimal information about all test XML file requests in the queue, and their status,
        /// and who started the run.
        /// </summary>
        IEnumerable<TaskDetails> GetRunningTasks();

        /// <summary>
        /// Shows the full information about a *single* test run - it doesn't have to be running, it could be complete.
        /// This includes the results of every test in the test file for the run.
        /// </summary>
        TaskDetails GetRunningTaskDetails(int taskId);

        /// <summary>
        /// Stops a test file XML request task in the queue, returning a message of whether the stop succeeded or not.
        /// </summary>
        string Stop(int id);

        /// <summary>
        /// Attempts to shut down all running tasks.
        /// </summary>
        List<string> StopAll();
    }
}