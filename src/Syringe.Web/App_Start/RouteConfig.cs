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
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
		   "TestCases",
		   "{action}/{filename}/{testCaseId}",
		   new { controller = "Home", action = "TestCase", filename = "", testCaseId = "" });
		}
	}
}
