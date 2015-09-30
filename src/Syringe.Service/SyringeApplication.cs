using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;
using Owin;
using StructureMap;
using Swashbuckle.Application;
using Syringe.Service.DependencyResolution;
using Syringe.Service.Parallel;
using WebApiContrib.IoC.StructureMap;

namespace Syringe.Service
{
	public class SyringeApplication
	{
		protected IDisposable WebApplication;

		public void Start()
		{
			WebApplication = WebApp.Start<SyringeApplication>("http://localhost:8086");
			RavenDbServer.Start();
		}

		public void Stop()
		{
			RavenDbServer.Stop();
			ParallelTestSessionQueue.Default.StopAll();
			// Manually eject the IHttpControllerActivator (DependencyResolver) to avoid a StackOverflowException
			ObjectFactory.EjectAllInstancesOf<IHttpControllerActivator>();
			WebApplication.Dispose();
		}

		public void Configuration(IAppBuilder application)
		{
			var config = new HttpConfiguration();
			config.EnableSwagger(swaggerConfig =>
			{
				swaggerConfig
					.SingleApiVersion("v1", "Syringe REST API")
					.Description("REST API for Syringe, used by the web UI.");

			}).EnableSwaggerUi();

			config.MapHttpAttributeRoutes();

			var container = IoC.Initialize();
			config.DependencyResolver = new StructureMapResolver(container);

			application.UseWebApi(config);
			application.MapSignalR(new HubConfiguration { EnableJSONP = true, Resolver = new StructureMapSignalRDependencyResolver(container) });
		}
	}
}