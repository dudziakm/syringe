using Microsoft.AspNet.SignalR;

namespace Syringe.Web.Hubs
{
    public class ProgressHub : Hub<IProgressHubClient>
    {
        public void SendProgress()
        {
            Clients.All.DoSomething();
        }
    }

    public interface IProgressHubClient
    {
        void DoSomething();

        void DoAnotherThing();
    }
}