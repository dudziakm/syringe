using Microsoft.AspNet.SignalR.Hubs;
using StructureMap;

namespace Syringe.Web.DependencyResolution
{
    /// <summary>
    /// Hub activator for SignalR (http://stackoverflow.com/questions/20705937/how-do-you-resolve-signalr-v2-0-with-structuremap-v2-6).
    /// </summary>
    public class HubActivator : IHubActivator
    {
        private readonly IContainer _container;

        public HubActivator(IContainer container)
        {
            _container = container;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            return (IHub) _container.GetInstance(descriptor.HubType);
        }
    }
}