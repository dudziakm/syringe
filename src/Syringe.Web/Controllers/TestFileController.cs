using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Syringe.Web.Controllers
{
    public class TestFileController : Controller
    {
        public TestFileController()
        {
            
        }

        // GET: TestFile
        public ActionResult Index()
        {
            return View();
        }
    }
}