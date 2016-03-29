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
	[Authorize]
	public class TestCaseController : Controller
	{
		private readonly ICaseService _casesClient;
		private readonly IUserContext _userContext;
		private readonly ITestCaseMapper _testCaseMapper;

		public TestCaseController(
			ICaseService casesClient,
			IUserContext userContext,
			ITestCaseMapper testCaseMapper)
		{
			_casesClient = casesClient;
			_userContext = userContext;
			_testCaseMapper = testCaseMapper;
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			AddPagingDataForBreadCrumb();
			base.OnActionExecuting(filterContext);
		}

		public ActionResult View(string filename, int pageNumber = 1, int noOfResults = 10)
		{
			CaseCollection testCases = _casesClient.GetTestCaseCollection(filename, _userContext.TeamName);
			var pagedTestCases = testCases.TestCases.GetPaged(noOfResults, pageNumber);

			TestFileViewModel caseList = new TestFileViewModel
			{
				PageNumbers = testCases.TestCases.GetPageNumbersToShow(noOfResults),
				TestCases = _testCaseMapper.BuildTestCases(pagedTestCases),
				Filename = filename,
				PageNumber = pageNumber,
				NoOfResults = noOfResults
			};

			return View("View", caseList);
		}

		public ActionResult Edit(string filename, int index)
		{
			Case testCase = _casesClient.GetTestCase(filename, _userContext.TeamName, index);
			TestCaseViewModel model = _testCaseMapper.BuildViewModel(testCase);

			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Edit(TestCaseViewModel model)
		{
			if (ModelState.IsValid)
			{
				Case testCase = _testCaseMapper.BuildCoreModel(model);
				_casesClient.EditTestCase(testCase, _userContext.TeamName);
				return RedirectToAction("View", new { filename = model.ParentFilename });
			}

			return View("Edit", model);
		}

		public ActionResult Add(string filename)
		{
			var model = new TestCaseViewModel { ParentFilename = filename };
			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Add(TestCaseViewModel model)
		{
			if (ModelState.IsValid)
			{
				Case testCase = _testCaseMapper.BuildCoreModel(model);
				_casesClient.CreateTestCase(testCase, _userContext.TeamName);
				return RedirectToAction("View", new { filename = model.ParentFilename });
			}

			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Delete(int index, string fileName)
		{
			_casesClient.DeleteTestCase(index, fileName, _userContext.TeamName);

			return RedirectToAction("View", new { filename = fileName });
		}

		public ActionResult AddVerification()
		{
			return PartialView("EditorTemplates/VerificationItemModel", new VerificationItemModel());
		}

		public ActionResult AddParseResponseItem()
		{
			return PartialView("EditorTemplates/ParseResponseItem", new Models.ParseResponseItem());
		}

		public ActionResult AddHeaderItem()
		{
			return PartialView("EditorTemplates/HeaderItem", new Models.HeaderItem());
		}

		private void AddPagingDataForBreadCrumb()
		{
			// Paging support for the breadcrumb trail
			ViewBag.PageNumber = Request.QueryString["pageNumber"];
			ViewBag.NoOfResults = Request.QueryString["noOfResults"];
		}

		public ActionResult EditXml(string fileName)
		{
			var model = new TestFileViewModel { Filename = fileName, TestCaseXml = _casesClient.GetXmlTestCaseCollection(fileName,_userContext.TeamName)};
			return View("EditXml", model);
		}
	}
}