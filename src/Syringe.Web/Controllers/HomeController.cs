using System.Collections.Generic;
using System.Web.Mvc;
using Syringe.Web.ApiClient;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	public class HomeController : Controller
	{
	    private readonly CasesClient _casesClient;

		public HomeController()
		{
		    _casesClient = new CasesClient();
		}

		public ActionResult Index()
		{
			// TODO: team name from the user context
			IEnumerable<string> files = _casesClient.ListForTeam("teamname");

			var model = new IndexViewModel();
			model.AddFiles(files);

			return View(model);
		}

		public ActionResult Run(string filename)
		{
			return View("Run", "", filename);
		}
		

	}
}