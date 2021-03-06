﻿using System;
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
	public class SyringeService
	{
		protected IDisposable WebApplication;
		private readonly IDependencyResolver _webDependencyResolver;
		private readonly IConfiguration _configuration;
		private readonly ITestFileQueue _testFileQueue;
		private readonly Microsoft.AspNet.SignalR.IDependencyResolver _signalRDependencyResolver;

		public SyringeService(
			IDependencyResolver webDependencyResolver,
			IConfiguration configuration,
			ITestFileQueue testFileQueue,
			Microsoft.AspNet.SignalR.IDependencyResolver signalRDependencyResolver)
		{
			_webDependencyResolver = webDependencyResolver;
			_configuration = configuration;
			_testFileQueue = testFileQueue;
			_signalRDependencyResolver = signalRDependencyResolver;
		}

		public void Start(string bindingUrl = "")
		{
			if (string.IsNullOrEmpty(bindingUrl))
				bindingUrl = _configuration.ServiceUrl;

			Console.WriteLine("bindingUrl: {0}", bindingUrl);

			WebApplication = WebApp.Start(bindingUrl, Configuration);
		}

		public void Stop()
		{
			_testFileQueue.StopAll();
			WebApplication.Dispose();
		}

		public void Configuration(IAppBuilder application)
		{
			var httpConfiguration = new HttpConfiguration();
			httpConfiguration.EnableSwagger(swaggerConfig =>
			{
				swaggerConfig
					.SingleApiVersion("v1", "Syringe REST API")
					.Description("REST API for Syringe, this is used by the web UI.");

			}).EnableSwaggerUi();

			httpConfiguration.MapHttpAttributeRoutes();
			httpConfiguration.DependencyResolver = _webDependencyResolver;

			var corsOptions = new CorsOptions
			{
				PolicyProvider = new CorsPolicyProvider
				{
					PolicyResolver = context =>
					{
						var policy = new CorsPolicy();
						// Allow CORS requests from the web frontend
						policy.Origins.Add(_configuration.WebsiteUrl);
						policy.AllowAnyMethod = true;
						policy.AllowAnyHeader = true;
						policy.SupportsCredentials = true;
						return Task.FromResult(policy);
					}
				}
			};

			//application.Map("", config =>
			//{
			//	config.Run(context =>
			//	{
			//		if (context.Request.Uri.PathAndQuery == "/")
			//		{
			//			return Task.Run(() => context.Response.Redirect("/swagger/ui/index"));
			//		}
			//	});
			//});

			application.Map("/signalr", config =>
            {
                config.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    EnableDetailedErrors = true,
                    Resolver = _signalRDependencyResolver
                };
                config.RunSignalR(hubConfiguration);
            });

            application.UseCors(corsOptions);
			application.UseWebApi(httpConfiguration);
		}
	}
}