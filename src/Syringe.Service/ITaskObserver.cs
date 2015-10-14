using Syringe.Service.Parallel;

namespace Syringe.Service
{
    public interface ITaskObserver
    {
        TaskMonitoringInfo StartMonitoringTask(int taskId);
    }
}