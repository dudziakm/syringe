using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using Owin;
using Raven.Client;
using Raven.Client.Document;
using Syringe.Core.Repositories.RavenDB;
using Syringe.Web;

[assembly: OwinStartup(typeof(Startup))]

namespace Syringe.Web
{
	public class Startup
	{
		public static IDocumentStore DocumentStore { get; set; }

		public void Configuration(IAppBuilder app)
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			ConfigureRavenDB();

			app.MapSignalR();
		}

		private void ConfigureRavenDB()
		{
			// TODO: IoC, singleton of DocumentStore (needs a wrapper)
			var ravenDbConfig = new RavenDBConfiguration();
			DocumentStore = new DocumentStore() { Url = ravenDbConfig.Url, DefaultDatabase = ravenDbConfig.DefaultDatabase };
			DocumentStore.Initialize();
		}
	}
}
