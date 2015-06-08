using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Syringe.Core;
using Syringe.Core.ApiClient;
using Syringe.Core.Security;
using Syringe.Web.Models;
using ParseResponseItem = Syringe.Web.Models.ParseResponseItem;

namespace Syringe.Web.Controllers
{
    public class TestCaseController : Controller
    {
        private readonly CasesClient _casesClient;
        private readonly IUserContext _userContext;

        public TestCaseController()
        {
            _casesClient = new CasesClient();
            _userContext = new UserContext();
        }

        public ActionResult View(string filename)
        {
            ViewData["Filename"] = filename;

            // TODO: tests
            CaseCollection testCases = _casesClient.GetTestCaseCollection(filename, _userContext.TeamName);
            var caseList = testCases.TestCases.Select(x => new TestCaseViewModel()
            {
                Id = x.Id,
                ShortDescription = x.ShortDescription,
                Url = x.Url
            });

            return View("View", caseList);
        }

        public ActionResult Edit(string filename, int testCaseId)
        {
            Case testCase = _casesClient.GetTestCase(filename, _userContext.TeamName, testCaseId);

            var verifications = new List<VerificationItem>();

            verifications.AddRange(testCase.VerifyPositives);
            verifications.AddRange(testCase.VerifyNegatives);

            var headerList = new List<HeaderItem>(testCase.Headers.Select(x => new HeaderItem { Key = x.Key, Value = x.Value }));

            var parsedResponses = new List<ParseResponseItem>(testCase.ParseResponses.Select(x => new ParseResponseItem { Description = x.Description, Regex = x.Regex }));


            var model = new TestCaseViewModel
            {
                Id = testCase.Id,
                ErrorMessage = testCase.ErrorMessage,
                Headers = headerList,
                LogRequest = testCase.LogRequest,
                LogResponse = testCase.LogResponse,
                LongDescription = testCase.LongDescription,
                Method = testCase.Method,
                ParseResponses = parsedResponses,
                PostBody = testCase.PostBody,
                PostType = testCase.PostType == PostType.GET.ToString() ? PostType.GET : PostType.POST,
                VerifyResponseCode = testCase.VerifyResponseCode,
                ShortDescription = testCase.ShortDescription,
                Sleep = testCase.Sleep,
                Url = testCase.Url,
                Verifications = verifications
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TestCaseViewModel model)
        {
            return View(model);
        }

        public ActionResult AddVerification(VerificationItemViewModel model)
        {
            var item = new VerificationItem
            {
                Description = model.Description,
                Regex = model.Regex,
                VerifyType = (VerifyType)Enum.Parse(typeof(VerifyType), model.VerifyType)
            };

            return PartialView("~/Views/TestCase/EditorTemplates/VerificationItem.cshtml", item);
        }


        public ActionResult AddParseResponseItem(ParseResponseItem model)
        {
            var item = new ParseResponseItem
            {
                Description = model.Description,
                Regex = model.Regex
            };

            return PartialView("~/Views/TestCase/EditorTemplates/ParseResponseItem.cshtml", item);
        }

        public ActionResult AddHeaderItem(HeaderItem model)
        {
            var item = new HeaderItem
            {
                Key = model.Key,
                Value = model.Value
            };

            return PartialView("~/Views/TestCase/EditorTemplates/HeaderItem.cshtml", item);
        }
    }
}