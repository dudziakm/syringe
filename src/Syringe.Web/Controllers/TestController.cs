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
	public class TestController : Controller
	{
		private readonly ITestService _testsClient;
		private readonly IUserContext _userContext;
		private readonly ITestFileMapper _testFileMapper;

		public TestController(
			ITestService testsClient,
			IUserContext userContext,
			ITestFileMapper testFileMapper)
		{
			_testsClient = testsClient;
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
			TestFile testCases = _testsClient.GetTestFile(filename, _userContext.DefaultBranchName);
			var pagedTestCases = testCases.Tests.GetPaged(noOfResults, pageNumber);

			TestFileViewModel caseList = new TestFileViewModel
			{
				PageNumbers = testCases.Tests.GetPageNumbersToShow(noOfResults),
				Tests = _testFileMapper.BuildTestCases(pagedTestCases),
				Filename = filename,
				PageNumber = pageNumber,
				NoOfResults = noOfResults
			};

			return View("View", caseList);
		}

		public ActionResult Edit(string filename, int position)
		{
			Test test = _testsClient.GetTest(filename, _userContext.TeamName, position);
			TestViewModel model = _testFileMapper.BuildViewModel(test);

			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Edit(TestViewModel model)
		{
			if (ModelState.IsValid)
			{
				Test test = _testFileMapper.BuildCoreModel(model);
				_testsClient.EditTest(test, _userContext.DefaultBranchName);
				return RedirectToAction("View", new { filename = model.Filename });
			}

			return View("Edit", model);
		}

		public ActionResult Add(string filename)
		{
			var model = new TestViewModel { Filename = filename };
			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Add(TestViewModel model)
		{
			if (ModelState.IsValid)
			{
				Test test = _testFileMapper.BuildCoreModel(model);
                _testsClient.CreateTest(test, _userContext.TeamName);
				return RedirectToAction("View", new { filename = model.Filename });
			}

			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Delete(int index, string fileName)
		{
			_testsClient.DeleteTest(index, fileName, _userContext.DefaultBranchName);

			return RedirectToAction("View", new { filename = fileName });
		}

		public ActionResult AddVerification()
		{
			return PartialView("EditorTemplates/AssertionViewModel", new AssertionViewModel());
		}

		public ActionResult AddParseResponseItem()
		{
			return PartialView("EditorTemplates/CapturedVariableItem", new CapturedVariableItem());
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
			var model = new TestFileViewModel { Filename = fileName, Xml = _testsClient.GetXml(fileName,_userContext.DefaultBranchName)};
			return View("EditXml", model);
		}
	}
}