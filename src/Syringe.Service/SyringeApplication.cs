using System;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using Swashbuckle.Application;
using Syringe.Core.Configuration;
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

			var corsOptions = new CorsOptions
			{
				PolicyProvider = new CorsPolicyProvider
				{
					PolicyResolver = context =>
					{
						var policy = new CorsPolicy();
						// Allow CORS requests from the web frontend
						policy.Origins.Add(container.GetInstance<IApplicationConfiguration>().WebsiteCorsUrl);
						policy.AllowAnyMethod = true;
						policy.AllowAnyHeader = true;
						policy.SupportsCredentials = true;
						return Task.FromResult(policy);
					}
				}
			};

			application.UseCors(corsOptions);
			application.UseWebApi(config);
			application.MapSignalR(new HubConfiguration { EnableDetailedErrors = true, Resolver = new StructureMapSignalRDependencyResolver(container) });
		}
	}
}