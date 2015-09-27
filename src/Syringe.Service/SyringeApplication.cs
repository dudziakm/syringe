using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using Swashbuckle.Application;
using Syringe.Service.Parallel;

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
			application.UseWebApi(config);
			application.MapSignalR();
		}
	}
}