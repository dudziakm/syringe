using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syringe.Core;
using Syringe.Core.Repositories;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
    public class TestCaseController : Controller
    {
        private readonly ICaseRepository _caseRepository;

        public TestCaseController(ICaseRepository caseRepository)
		{
			_caseRepository = caseRepository;
		}

        public TestCaseController() : this(new CaseRepository()) { }
		

        public ActionResult Index(string filename, int testCaseId)
        {
            var testCase = _caseRepository.GetTestCase(filename, testCaseId);

            testCase.VerifyNegatives.AddRange(testCase.VerifyPositives);

            var model = new TestCaseViewModel
            {
                Id = testCase.Id,
                ErrorMessage = testCase.ErrorMessage,
                Headers = new List<Models.Header>(testCase.Headers.Select(x => new Models.Header { Key = x.Key, Value = x.Value })),
                LogRequest = testCase.LogRequest,
                LogResponse = testCase.LogResponse,
                LongDescription = testCase.LongDescription,
                Method = testCase.Method,
                ParseResponses = testCase.ParseResponses,
                PostBody = testCase.PostBody,
                PostType = testCase.PostType == PostType.GET.ToString() ? PostType.GET : PostType.POST,
                VerifyResponseCode = testCase.VerifyResponseCode,
                ShortDescription = testCase.ShortDescription,
                Sleep = testCase.Sleep,
                Url = testCase.Url,
                Verifications = testCase.VerifyNegatives
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(TestCaseViewModel model)
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
    }
}