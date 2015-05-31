using System;
using System.Web.Mvc;
using RestSharp;
using Syringe.Web.Client;

namespace Syringe.Web.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Run(string filename)
		{
			return View("Run", "", filename);
		}
	}
}