using System;
using System.Web.Mvc;
using Syringe.Core.Extensions;
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

        public ActionResult View(string filename, int pageNumber = 1, int noOfResults = 10)
        {
            CaseCollection testCases = _casesClient.GetTestCaseCollection(filename, _userContext.TeamName);
            var pagedTestCases = testCases.TestCases.GetPaged(noOfResults, pageNumber);

            TestFileViewModel caseList = new TestFileViewModel
            {
                TotalCases = testCases.TestCases.GetPageNumbersToShow(noOfResults),
                TestCases = _testCaseViewModelBuilder.BuildTestCases(pagedTestCases),
                Filename = filename,
                PageNumber = pageNumber,
                NoOfResults = noOfResults
            };

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
                _casesClient.EditTestCase(testCase, _userContext.TeamName);
                return RedirectToAction("View", new { filename = model.ParentFilename });
            }

            return View("Edit", model);
        }

        public ActionResult Add(string filename)
        {
            var model = new TestCaseViewModel { ParentFilename = filename };
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(TestCaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                Case testCase = _testCaseCoreModelBuilder.Build(model);
                _casesClient.CreateTestCase(testCase, _userContext.TeamName);
                return RedirectToAction("View", new { filename = model.ParentFilename });
            }

            return View("Add", model);
        }

        public ActionResult Delete(int testCaseId, string fileName)
        {
            var deleteTestCase = _casesClient.DeleteTestCase(testCaseId, fileName, _userContext.TeamName);

            if (deleteTestCase)
            {
                return RedirectToAction("View", new { filename = fileName });
            }

            return View();
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