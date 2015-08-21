using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Syringe.Client;
using Syringe.Core;
using Syringe.Core.Security;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;
using HeaderItem = Syringe.Core.HeaderItem;


namespace Syringe.Web.Controllers
{
	public class TestCaseController : Controller
	{
		private readonly CasesClient _casesClient;
		private readonly IUserContext _userContext;
		private readonly ITestCaseViewModelBuilder _testCaseViewModelBuilder;

		public TestCaseController()
		{
			_casesClient = new CasesClient();
			_userContext = new UserContext();
			_testCaseViewModelBuilder = new TestCaseViewModelBuilder();
		}

		public ActionResult View(string filename)
		{
			ViewData["Filename"] = filename;

			// TODO: tests
			CaseCollection testCases = _casesClient.GetTestCaseCollection(filename, _userContext.TeamName);
			var caseList = _testCaseViewModelBuilder.BuildTestCases(testCases);

			return View("View", caseList);
		}

		public ActionResult Edit(string filename, int testCaseId)
		{
			Case testCase = _casesClient.GetTestCase(filename, _userContext.TeamName, testCaseId);
			var model = _testCaseViewModelBuilder.BuildTestCase(testCase);

			return View(model);
		}

		[HttpPost]
		public ActionResult Edit(TestCaseViewModel model)
		{
			//todo move to builder
			var testCase = new Case
			{
				Id = model.Id,
				ErrorMessage = model.ErrorMessage,
				Headers = model.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList(),
				LogRequest = model.LogRequest,
				LogResponse = model.LogResponse,
				LongDescription = model.LongDescription,
				Method = model.Method,
				ParentFilename = model.ParentFilename,
				ParseResponses = model.ParseResponses.Select(x => new Core.ParseResponseItem(x.Description, x.Regex)).ToList(),
				PostBody = model.PostBody,
				VerifyPositives = model.Verifications.Where(x => x.VerifyType == VerifyType.Positive).Select(x => new Core.VerificationItem(x.Description, x.Regex, x.VerifyType)).ToList(),
				VerifyNegatives = model.Verifications.Where(x => x.VerifyType == VerifyType.Negative).Select(x => new Core.VerificationItem(x.Description, x.Regex, x.VerifyType)).ToList(),
				ShortDescription = model.ShortDescription,
				Url = model.Url,
				Sleep = model.Sleep,
				PostType = model.PostType.ToString(),
				VerifyResponseCode = model.VerifyResponseCode,

			};
			var addTestCase = _casesClient.AddTestCase(testCase, _userContext.TeamName);

			return RedirectToAction("View", new { filename = model.ParentFilename });
		}

		public ActionResult AddVerification(Models.VerificationItem model)
		{
			var item = new Models.VerificationItem
			{
				Description = model.Description,
				Regex = model.Regex,
				VerifyType = (VerifyType)Enum.Parse(typeof(VerifyType), model.VerifyTypeValue)
			};

			return PartialView("~/Views/TestCase/EditorTemplates/VerificationItem.cshtml", item);
		}

		public ActionResult AddParseResponseItem(Models.ParseResponseItem model)
		{
			var item = new Models.ParseResponseItem
			{
				Description = model.Description,
				Regex = model.Regex
			};

			return PartialView("~/Views/TestCase/EditorTemplates/ParseResponseItem.cshtml", item);
		}

		public ActionResult AddHeaderItem(Models.HeaderItem model)
		{
			var item = new Models.HeaderItem
			{
				Key = model.Key,
				Value = model.Value
			};

			return PartialView("~/Views/TestCase/EditorTemplates/HeaderItem.cshtml", item);
		}
	}
}