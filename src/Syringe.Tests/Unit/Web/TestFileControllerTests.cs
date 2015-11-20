using System;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.TestCases;
using Syringe.Web.Controllers;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class TestFileControllerTests
    {
        private TestFileController _testCaseController;
        private Mock<ICaseService> _caseServiceMock;
        private Mock<IUserContext> _userContextMock;

        [SetUp]
        public void Setup()
        {
            _caseServiceMock = new Mock<ICaseService>();
            _userContextMock = new Mock<IUserContext>();

            _userContextMock.Setup(x => x.TeamName).Returns("Team");
            _caseServiceMock.Setup(x => x.GetTestCaseCollection(It.IsAny<string>(), _userContextMock.Object.TeamName)).Returns(new CaseCollection());
            _caseServiceMock.Setup(x => x.UpdateTestFile(It.IsAny<CaseCollection>(), It.IsAny<string>())).Returns(true);
            _caseServiceMock.Setup(x => x.CreateTestFile(It.IsAny<CaseCollection>(), It.IsAny<string>())).Returns(true);

            _testCaseController = new TestFileController(_caseServiceMock.Object, _userContextMock.Object);
        }

        [Test]
        public void Add_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testCaseController.Add() as ViewResult;

            // then
            Assert.AreEqual("Add", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }
        [Test]
        public void Add_should_redirect_to_view_when_validation_succeeded()
        {
            // given
            _testCaseController.ModelState.Clear();

            // when
            var redirectToRouteResult = _testCaseController.Add(new TestFileViewModel()) as RedirectToRouteResult;

            // then
            _caseServiceMock.Verify(x => x.CreateTestFile(It.IsAny<CaseCollection>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual("Home", redirectToRouteResult.RouteValues["controller"]);
        }

        [Test]
        public void Add_should_return_correct_view_and_model_when_validation_failed_on_post()
        {
            // given
            _testCaseController.ModelState.AddModelError("error", "error");

            // when
            var viewResult = _testCaseController.Add(new TestFileViewModel()) as ViewResult;

            // then
            _caseServiceMock.Verify(x => x.CreateTestFile(It.IsAny<CaseCollection>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual("Add", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Add_should_return_correct_view_and_model_when_update_failed()
        {
            // given
            _caseServiceMock.Setup(x => x.CreateTestFile(It.IsAny<CaseCollection>(), It.IsAny<string>())).Returns(false);

            // when
            var viewResult = _testCaseController.Add(new TestFileViewModel()) as ViewResult;

            // then
            _caseServiceMock.Verify(x => x.CreateTestFile(It.IsAny<CaseCollection>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("Add", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }


        [Test]
        public void Update_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testCaseController.Update(It.IsAny<string>()) as ViewResult;

            // then
            _caseServiceMock.Verify(x => x.GetTestCaseCollection(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("Update", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Update_should_redirect_to_view_when_validation_succeeded()
        {
            // given + when
            var redirectToRouteResult = _testCaseController.Update(new TestFileViewModel()) as RedirectToRouteResult;

            // then
            _caseServiceMock.Verify(x => x.UpdateTestFile(It.IsAny<CaseCollection>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["action"]);
            Assert.AreEqual("Home", redirectToRouteResult.RouteValues["controller"]);
        }

        [Test]
        public void Update_should_return_correct_view_and_model_when_validation_failed_on_post()
        {
            // given
            _testCaseController.ModelState.AddModelError("error", "error");

            // when
            var viewResult = _testCaseController.Update(new TestFileViewModel()) as ViewResult;

            // then
            _caseServiceMock.Verify(x => x.UpdateTestFile(It.IsAny<CaseCollection>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual("Update", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Update_should_return_correct_view_and_model_when_update_failed()
        {
            // given
            _caseServiceMock.Setup(x => x.UpdateTestFile(It.IsAny<CaseCollection>(), It.IsAny<string>())).Returns(false);

            // when
            var viewResult = _testCaseController.Update(new TestFileViewModel()) as ViewResult;

            // then
            _caseServiceMock.Verify(x => x.UpdateTestFile(It.IsAny<CaseCollection>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("Update", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void AddHeaderItem_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testCaseController.AddVariableItem() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/VariableItem", viewResult.ViewName);
            Assert.IsInstanceOf<VariableItem>(viewResult.Model);
        }
    }
}
