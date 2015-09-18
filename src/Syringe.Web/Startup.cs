using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using Syringe.Core.Configuration;
using Syringe.Web;
using Syringe.Web.Hubs;

[assembly: OwinStartup(typeof(Startup))]

namespace Syringe.Web
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			// TODO: http://stackoverflow.com/questions/9790433/signalr-dependency-injection-questions
			GlobalHost.DependencyResolver.Register(
						typeof(ProgressHub),
						() => new ProgressHub(new SignalRProgressNotifier(new ApplicationConfig())));

			app.MapSignalR();
		}
	}
}
