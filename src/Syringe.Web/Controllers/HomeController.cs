using System.Collections.Generic;
using System.Web.Mvc;
using Syringe.Core.ApiClient;
using Syringe.Core.Security;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	public class HomeController : Controller
	{
	    private readonly CasesClient _casesClient;
		private readonly IUserContext _userContext;

		public HomeController()
		{
		    _casesClient = new CasesClient();
			_userContext = new UserContext();
		}

		public ActionResult Index()
		{
			// TODO: team name from the user context
			IEnumerable<string> files = _casesClient.ListFilesForTeam(_userContext.TeamName);

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