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
using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace Syringe.Service
{
	public class SyringeApplication
	{
		protected IDisposable WebApplication;
		private readonly IDependencyResolver _webDependencyResolver;
		private readonly IApplicationConfiguration _applicationConfiguration;
		private readonly ITestSessionQueue _testSessionQueue;
		private readonly Microsoft.AspNet.SignalR.IDependencyResolver _signalRDependencyResolver;

		public SyringeApplication(
			IDependencyResolver webDependencyResolver,
			IApplicationConfiguration applicationConfiguration,
			ITestSessionQueue testSessionQueue,
			Microsoft.AspNet.SignalR.IDependencyResolver signalRDependencyResolver)
		{
			_webDependencyResolver = webDependencyResolver;
			_applicationConfiguration = applicationConfiguration;
			_testSessionQueue = testSessionQueue;
			_signalRDependencyResolver = signalRDependencyResolver;
		}

		public void Start()
		{
			WebApplication = WebApp.Start("http://*:8086", Configuration);
		}

		public void Stop()
		{
			_testSessionQueue.StopAll();
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

			config.DependencyResolver = _webDependencyResolver;

			var corsOptions = new CorsOptions
			{
				PolicyProvider = new CorsPolicyProvider
				{
					PolicyResolver = context =>
					{
						var policy = new CorsPolicy();
						// Allow CORS requests from the web frontend
						policy.Origins.Add(_applicationConfiguration.WebsiteCorsUrl);
						policy.AllowAnyMethod = true;
						policy.AllowAnyHeader = true;
						policy.SupportsCredentials = true;
						return Task.FromResult(policy);
					}
				}
			};

			application.UseCors(corsOptions);
			application.UseWebApi(config);
			application.MapSignalR(new HubConfiguration { EnableDetailedErrors = true, Resolver = _signalRDependencyResolver });
		}
	}
}