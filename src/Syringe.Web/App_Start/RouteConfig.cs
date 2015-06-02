using System.Web.Mvc;
using System.Web.Routing;

namespace Syringe.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



		    routes.MapRoute(
		        "TestCases",
		        "TestCases/{filename}/{testCaseId}",
		        new {controller = "TestCase", action = "Index", filename = "", testCaseId = 0});

           routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
