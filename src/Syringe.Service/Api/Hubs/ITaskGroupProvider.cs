namespace Syringe.Service.Api.Hubs
{
    public interface ITaskGroupProvider
    {
        string GetGroupForTask(int taskId);
    }
}