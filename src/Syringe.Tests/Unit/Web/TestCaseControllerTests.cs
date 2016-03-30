using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tests;
using Syringe.Web.Controllers;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class TestCaseControllerTests
    {
        private TestController _testController;
        private Mock<ITestService> ICaseServiceMock;
        private Mock<IUserContext> IUserContextMock;
        private Mock<ITestFileMapper> ITestCaseMapperMock;

        [SetUp]
        public void Setup()
        {
            ICaseServiceMock = new Mock<ITestService>();
            IUserContextMock = new Mock<IUserContext>();
            ITestCaseMapperMock = new Mock<ITestFileMapper>();

            IUserContextMock.Setup(x => x.DefaultBranchName).Returns("master");
            ITestCaseMapperMock.Setup(x => x.BuildTestCases(It.IsAny<IEnumerable<Test>>()));
            ITestCaseMapperMock.Setup(x => x.BuildViewModel(It.IsAny<Test>())).Returns(new TestViewModel());
            ICaseServiceMock.Setup(x => x.GetTestFile(It.IsAny<string>(), IUserContextMock.Object.DefaultBranchName)).Returns(new TestFile());
            ICaseServiceMock.Setup(x => x.GetTest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
            ICaseServiceMock.Setup(x => x.DeleteTest(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));
            ICaseServiceMock.Setup(x => x.EditTest(It.IsAny<Test>(), It.IsAny<string>()));
            ICaseServiceMock.Setup(x => x.CreateTest(It.IsAny<Test>(), It.IsAny<string>()));

            _testController = new TestController(ICaseServiceMock.Object, IUserContextMock.Object, ITestCaseMapperMock.Object);
        }

        [Test]
        public void View_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testController.View(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            // then
            ICaseServiceMock.Verify(x => x.GetTestFile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            ITestCaseMapperMock.Verify(x => x.BuildTestCases(It.IsAny<IEnumerable<Test>>()), Times.Once);
            Assert.AreEqual("View", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Edit_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testController.Edit(It.IsAny<string>(), It.IsAny<int>()) as ViewResult;

            // then
            ICaseServiceMock.Verify(x => x.GetTest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
            ITestCaseMapperMock.Verify(x => x.BuildViewModel(It.IsAny<Test>()), Times.Once);
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<TestViewModel>(viewResult.Model);
        }

        [Test]
        public void Edit_should_redirect_to_view_when_validation_succeeded()
        {
            // given
            _testController.ModelState.Clear();
            
            // when
            var redirectToRouteResult = _testController.Edit(new TestViewModel()) as RedirectToRouteResult;

            // then
            ITestCaseMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Once);
            ICaseServiceMock.Verify(x => x.EditTest(It.IsAny<Test>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("View", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Edit_should_return_correct_view_and_model_when_validation_failed_on_post()
        {
            // given
            _testController.ModelState.AddModelError("error", "error");

            // when
            var viewResult = _testController.Edit(new TestViewModel()) as ViewResult;

            // then
            ITestCaseMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Never);
            ICaseServiceMock.Verify(x => x.EditTest(It.IsAny<Test>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<TestViewModel>(viewResult.Model);
        }

        [Test]
        public void EditXml_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testController.EditXml(It.IsAny<string>()) as ViewResult;

            // then 
            ICaseServiceMock.Verify(x => x.GetXml(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("EditXml", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Delete_should_return_correct_redirection_to_view()
        {
            // given + when
            var redirectToRouteResult = _testController.Delete(It.IsAny<int>(), It.IsAny<string>()) as RedirectToRouteResult;

            // then
            ICaseServiceMock.Verify(x => x.DeleteTest(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("View", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Add_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = _testController.Add(It.IsAny<string>()) as ViewResult;

            // then
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<TestViewModel>(viewResult.Model);
        }

        [Test]
        public void Add_should_redirect_to_view_when_validation_succeeded()
        {
            // given
            _testController.ModelState.Clear();

            // when
            var redirectToRouteResult = _testController.Add(new TestViewModel()) as RedirectToRouteResult;

            // then
            ITestCaseMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Once);
            ICaseServiceMock.Verify(x => x.CreateTest(It.IsAny<Test>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("View", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Add_should_return_correct_view_and_model_when_validation_failed_on_post()
        {
            // given
            _testController.ModelState.AddModelError("error", "error");

            // when
            var viewResult = _testController.Add(new TestViewModel()) as ViewResult;

            // then
            ITestCaseMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Never);
            ICaseServiceMock.Verify(x => x.CreateTest(It.IsAny<Test>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual("Edit", viewResult.ViewName);

        }

        [Test]
        public void AddVerification_should_return_correct_view()
        {
            // given + when
            var viewResult = _testController.AddVerification() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/AssertionViewModel", viewResult.ViewName);
            Assert.IsInstanceOf<AssertionViewModel>(viewResult.Model);
        }

        [Test]
        public void AddParseResponseItem_should_return_correct_view()
        {
            // given + when
            var viewResult = _testController.AddParseResponseItem() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/VariableViewModel", viewResult.ViewName);
            Assert.IsInstanceOf<Syringe.Web.Models.CapturedVariableItem>(viewResult.Model);
        }

        [Test]
        public void AddHeaderItem_should_return_correct_view()
        {
            // given + when
            var viewResult = _testController.AddHeaderItem() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/HeaderItem", viewResult.ViewName);
            Assert.IsInstanceOf<Syringe.Web.Models.HeaderItem>(viewResult.Model);
        }
    }
}
