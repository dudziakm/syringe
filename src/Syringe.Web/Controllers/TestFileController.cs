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
            var model = new TestFileViewModel();
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
                    Variables = model.Variables != null ? model.Variables.Select(x => new Variable(x.Name, x.Value, x.Environment)).ToList() : new List<Variable>()
                };

                bool createdTestFile = _casesClient.CreateTestFile(caseCollection, _userContext.TeamName);
                if (createdTestFile)
                    return RedirectToAction(" Position", "Home");
            }

            return View("Add", model);
        }

        public ActionResult Update(string fileName)
        {
            CaseCollection testCaseCollection = _casesClient.GetTestCaseCollection(fileName, _userContext.TeamName);

            TestFileViewModel model = new TestFileViewModel
            {
                Filename = fileName,
                Variables =
                    testCaseCollection.Variables.Select(x => new TestFileVariableModel { Name = x.Name, Value = x.Value, Environment = x.Environment.Name}).ToList()
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
                    Variables = model.Variables != null ? model.Variables.Select(x => new Variable(x.Name, x.Value, x.Environment)).ToList() : new List<Variable>()
				};

                bool updateTestFile = _casesClient.UpdateTestFile(caseCollection, _userContext.TeamName);
                if (updateTestFile)
                    return RedirectToAction(" Position", "Home");
            }

            return View("Update", model);
        }

        public ActionResult AddVariableItem()
        {
            return PartialView("EditorTemplates/TestFileVariableModel", new TestFileVariableModel());
        }
	}
}