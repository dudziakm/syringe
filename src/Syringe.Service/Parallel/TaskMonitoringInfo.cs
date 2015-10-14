namespace Syringe.Service.Parallel
{
    public class TaskMonitoringInfo
    {
        public TaskMonitoringInfo(int totalCases)
        {
            TotalCases = totalCases;
        }

        public int TotalCases { get; private set; }
    }
}