using System;
using System.Web.Mvc;
using Syringe.Core.Extensions;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;


namespace Syringe.Web.Controllers
{
	[Authorize]
	public class TestCaseController : Controller
	{
		private readonly ICaseService _casesClient;
		private readonly IUserContext _userContext;
		private readonly ITestFileMapper _testFileMapper;

		public TestCaseController(
			ICaseService casesClient,
			IUserContext userContext,
			ITestFileMapper testFileMapper)
		{
			_casesClient = casesClient;
			_userContext = userContext;
			_testFileMapper = testFileMapper;
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			AddPagingDataForBreadCrumb();
			base.OnActionExecuting(filterContext);
		}

		public ActionResult View(string filename, int pageNumber = 1, int noOfResults = 10)
		{
			TestFile testCases = _casesClient.GetTestCaseCollection(filename, _userContext.TeamName);
			var pagedTestCases = testCases.Tests.GetPaged(noOfResults, pageNumber);

			TestFileViewModel caseList = new TestFileViewModel
			{
				PageNumbers = testCases.Tests.GetPageNumbersToShow(noOfResults),
				TestCases = _testFileMapper.BuildTestCases(pagedTestCases),
				Filename = filename,
				PageNumber = pageNumber,
				NoOfResults = noOfResults
			};

			return View("View", caseList);
		}

		public ActionResult Edit(string filename, Guid testCaseId)
		{
			Test testTest = _casesClient.GetTestCase(filename, _userContext.TeamName, testCaseId);
			TestViewModel model = _testFileMapper.BuildViewModel(testTest);

			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Edit(TestViewModel model)
		{
			if (ModelState.IsValid)
			{
				Test testTest = _testFileMapper.BuildCoreModel(model);
				_casesClient.EditTestCase(testTest, _userContext.TeamName);
				return RedirectToAction("View", new { filename = model.ParentFilename });
			}

			return View("Edit", model);
		}

		public ActionResult Add(string filename)
		{
			var model = new TestViewModel { ParentFilename = filename, Id = Guid.NewGuid() };
			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Add(TestViewModel model)
		{
			if (ModelState.IsValid)
			{
				Test testTest = _testFileMapper.BuildCoreModel(model);
				_casesClient.CreateTestCase(testTest, _userContext.TeamName);
				return RedirectToAction("View", new { filename = model.ParentFilename });
			}

			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Delete(Guid testCaseId, string fileName)
		{
			_casesClient.DeleteTestCase(testCaseId, fileName, _userContext.TeamName);

			return RedirectToAction("View", new { filename = fileName });
		}

		public ActionResult AddVerification()
		{
			return PartialView("EditorTemplates/AssertionViewModel", new AssertionViewModel());
		}

		public ActionResult AddParseResponseItem()
		{
			return PartialView("EditorTemplates/ParseResponseItem", new Models.CapturedVariableItem());
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