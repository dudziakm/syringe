using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.TestCases;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	[Authorize]
	public class TestFileController : Controller
    {
        private readonly ICaseService _casesClient;
        private readonly IUserContext _userContext;

        public TestFileController(ICaseService casesClient, IUserContext userContext)
        {
            _casesClient = casesClient;
            _userContext = userContext;
        }

        public ActionResult Add()
        {
            TestFileViewModel model = new TestFileViewModel();

            return View("Add", model);
        }

        [HttpPost]
        public ActionResult Add(TestFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var caseCollection = new CaseCollection
                {
                    Filename = model.Filename,
                    Variables = model.Variables != null ? model.Variables.Select(x => new Variable(x.Key, x.Value)).ToList() : new List<Variable>()
                };

                var createdTestFile = _casesClient.CreateTestFile(caseCollection, _userContext.TeamName);
                if (createdTestFile)
                    return RedirectToAction("Index", "Home");
            }

            return View("Add", model);
        }

        public ActionResult Update(string fileName)
        {
            var testCaseCollection = _casesClient.GetTestCaseCollection(fileName, _userContext.TeamName);

            TestFileViewModel model = new TestFileViewModel
            {
                Filename = fileName,
                Variables =
                    testCaseCollection.Variables.Select(x => new VariableItem { Key = x.Name, Value = x.Value }).ToList()
            };

            return View("Update", model);
        }

        [HttpPost]
        public ActionResult Update(TestFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var caseCollection = new CaseCollection
                {
                    Filename = model.Filename,
                    Variables = model.Variables != null ? model.Variables.Select(x => new Variable(x.Key, x.Value)).ToList() : new List<Variable>()
				};

                var updateTestFile = _casesClient.UpdateTestFile(caseCollection, _userContext.TeamName);
                if (updateTestFile)
                    return RedirectToAction("Index", "Home");
            }

            return View("Update", model);
        }

        public ActionResult AddVariableItem()
        {
            return PartialView("EditorTemplates/VariableItem", new VariableItem());
        }
	}
}