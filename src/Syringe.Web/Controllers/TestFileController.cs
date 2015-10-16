using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.TestCases;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
    public class TestFileController : Controller
    {
        private readonly ICaseService _casesClient;
        private readonly IUserContext _userContext;

        public TestFileController(ICaseService casesClient, IUserContext userContext)
        {
            _casesClient = casesClient;
            _userContext = userContext;
        }

        // GET: TestFile
        public ActionResult Add()
        {
            TestFileViewModel model = new TestFileViewModel();

            return View(model);
        }

        // GET: TestFile
        [HttpPost]
        public ActionResult Add(TestFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var caseCollection = new CaseCollection
                {
                    Filename = model.Filename,
                    Variables = model.Variables!= null ? model.Variables.ToDictionary(x => x.Key, x => x.Value) : new Dictionary<string, string>()
                };

                var testFile = _casesClient.CreateTestFile(caseCollection, _userContext.TeamName);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public ActionResult AddVariableItem(VariableItem model)
        {
            var item = new VariableItem
            {
                Key = model.Key,
                Value = model.Value
            };

            return PartialView("EditorTemplates/VariableItem", item);
        }
    }
}