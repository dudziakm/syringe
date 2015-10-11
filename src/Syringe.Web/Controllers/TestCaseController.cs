using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.TestCases;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;


namespace Syringe.Web.Controllers
{
    public class TestCaseController : Controller
    {
        private readonly ICaseService _casesClient;
        private readonly IUserContext _userContext;
        private readonly ITestCaseViewModelBuilder _testCaseViewModelBuilder;
        private readonly ITestCaseCoreModelBuilder _testCaseCoreModelBuilder;

        public TestCaseController(
            ICaseService casesClient,
            IUserContext userContext,
            ITestCaseViewModelBuilder testCaseViewModelBuilder,
            ITestCaseCoreModelBuilder testCaseCoreModelBuilder)
        {
            _casesClient = casesClient;
            _userContext = userContext;
            _testCaseViewModelBuilder = testCaseViewModelBuilder;
            _testCaseCoreModelBuilder = testCaseCoreModelBuilder;
        }

        public ActionResult View(string filename, int pageNumber = 1, int take = 10)
        {
            ViewData["Filename"] = filename;

            // TODO: tests
            CaseCollection testCases = _casesClient.GetTestCaseCollection(filename, _userContext.TeamName);

            ViewData["TotalCases"] = testCases.TestCases.Count() / take;
            ViewData["PageNumber"] = pageNumber;

            testCases.TestCases = testCases.TestCases.Skip((pageNumber - 1) * take).Take(take);

            IEnumerable<TestCaseViewModel> caseList = _testCaseViewModelBuilder.BuildTestCases(testCases);

            return View("View", caseList);
        }

        public ActionResult Edit(string filename, int testCaseId)
        {
            Case testCase = _casesClient.GetTestCase(filename, _userContext.TeamName, testCaseId);
            TestCaseViewModel model = _testCaseViewModelBuilder.BuildTestCase(testCase);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TestCaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                Case testCase = _testCaseCoreModelBuilder.Build(model);
                _casesClient.AddTestCase(testCase, _userContext.TeamName);
                return RedirectToAction("View", new { filename = model.ParentFilename });
            }

            return View("Edit", model);
        }



        public ActionResult AddVerification(VerificationItemModel model)
        {
            var item = new VerificationItemModel
            {
                Description = model.Description,
                Regex = model.Regex,
                VerifyType = (VerifyType)Enum.Parse(typeof(VerifyType), model.VerifyTypeValue)
            };

            return PartialView("EditorTemplates/VerificationItemModel", item);
        }

        public ActionResult AddParseResponseItem(Models.ParseResponseItem model)
        {
            var item = new Models.ParseResponseItem
            {
                Description = model.Description,
                Regex = model.Regex
            };

            return PartialView("EditorTemplates/ParseResponseItem", item);
        }

        public ActionResult AddHeaderItem(Models.HeaderItem model)
        {
            var item = new Models.HeaderItem
            {
                Key = model.Key,
                Value = model.Value
            };

            return PartialView("EditorTemplates/HeaderItem", item);
        }
    }
}