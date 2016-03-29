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
        private TestCaseController testCaseController;
        private Mock<ICaseService> ICaseServiceMock;
        private Mock<IUserContext> IUserContextMock;
        private Mock<ITestFileMapper> ITestCaseMapperMock;

        [SetUp]
        public void Setup()
        {
            ICaseServiceMock = new Mock<ICaseService>();
            IUserContextMock = new Mock<IUserContext>();
            ITestCaseMapperMock = new Mock<ITestFileMapper>();

            IUserContextMock.Setup(x => x.TeamName).Returns("Team");
            ITestCaseMapperMock.Setup(x => x.BuildTestCases(It.IsAny<IEnumerable<Test>>()));
            ITestCaseMapperMock.Setup(x => x.BuildViewModel(It.IsAny<Test>())).Returns(new TestViewModel());
            ICaseServiceMock.Setup(x => x.GetTestCaseCollection(It.IsAny<string>(), IUserContextMock.Object.TeamName)).Returns(new TestFile());
            ICaseServiceMock.Setup(x => x.GetTestCase(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()));
            ICaseServiceMock.Setup(x => x.DeleteTestCase(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
            ICaseServiceMock.Setup(x => x.EditTestCase(It.IsAny<Test>(), It.IsAny<string>()));
            ICaseServiceMock.Setup(x => x.CreateTestCase(It.IsAny<Test>(), It.IsAny<string>()));

            testCaseController = new TestCaseController(ICaseServiceMock.Object, IUserContextMock.Object, ITestCaseMapperMock.Object);
        }

        [Test]
        public void View_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = testCaseController.View(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            // then
            ICaseServiceMock.Verify(x => x.GetTestCaseCollection(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            ITestCaseMapperMock.Verify(x => x.BuildTestCases(It.IsAny<IEnumerable<Test>>()), Times.Once);
            Assert.AreEqual("View", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Edit_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = testCaseController.Edit(It.IsAny<string>(), It.IsAny<Guid>()) as ViewResult;

            // then
            ICaseServiceMock.Verify(x => x.GetTestCase(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
            ITestCaseMapperMock.Verify(x => x.BuildViewModel(It.IsAny<Test>()), Times.Once);
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<TestViewModel>(viewResult.Model);
        }

        [Test]
        public void Edit_should_redirect_to_view_when_validation_succeeded()
        {
            // given
            testCaseController.ModelState.Clear();
            
            // when
            var redirectToRouteResult = testCaseController.Edit(new TestViewModel()) as RedirectToRouteResult;

            // then
            ITestCaseMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Once);
            ICaseServiceMock.Verify(x => x.EditTestCase(It.IsAny<Test>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("View", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Edit_should_return_correct_view_and_model_when_validation_failed_on_post()
        {
            // given
            testCaseController.ModelState.AddModelError("error", "error");

            // when
            var viewResult = testCaseController.Edit(new TestViewModel()) as ViewResult;

            // then
            ITestCaseMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Never);
            ICaseServiceMock.Verify(x => x.EditTestCase(It.IsAny<Test>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<TestViewModel>(viewResult.Model);
        }

        [Test]
        public void EditXml_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = testCaseController.EditXml(It.IsAny<string>()) as ViewResult;

            // then 
            ICaseServiceMock.Verify(x => x.GetXmlTestCaseCollection(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("EditXml", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Delete_should_return_correct_redirection_to_view()
        {
            // given + when
            var redirectToRouteResult = testCaseController.Delete(It.IsAny<Guid>(), It.IsAny<string>()) as RedirectToRouteResult;

            // then
            ICaseServiceMock.Verify(x => x.DeleteTestCase(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("View", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Add_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = testCaseController.Add(It.IsAny<string>()) as ViewResult;

            // then
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<TestViewModel>(viewResult.Model);
        }

        [Test]
        public void Add_should_redirect_to_view_when_validation_succeeded()
        {
            // given
            testCaseController.ModelState.Clear();

            // when
            var redirectToRouteResult = testCaseController.Add(new TestViewModel()) as RedirectToRouteResult;

            // then
            ITestCaseMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Once);
            ICaseServiceMock.Verify(x => x.CreateTestCase(It.IsAny<Test>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual("View", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Add_should_return_correct_view_and_model_when_validation_failed_on_post()
        {
            // given
            testCaseController.ModelState.AddModelError("error", "error");

            // when
            var viewResult = testCaseController.Add(new TestViewModel()) as ViewResult;

            // then
            ITestCaseMapperMock.Verify(x => x.BuildCoreModel(It.IsAny<TestViewModel>()), Times.Never);
            ICaseServiceMock.Verify(x => x.CreateTestCase(It.IsAny<Test>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual("Edit", viewResult.ViewName);

        }

        [Test]
        public void AddVerification_should_return_correct_view()
        {
            // given + when
            var viewResult = testCaseController.AddVerification() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/AssertionViewModel", viewResult.ViewName);
            Assert.IsInstanceOf<AssertionViewModel>(viewResult.Model);
        }

        [Test]
        public void AddParseResponseItem_should_return_correct_view()
        {
            // given + when
            var viewResult = testCaseController.AddParseResponseItem() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/ParseResponseItem", viewResult.ViewName);
            Assert.IsInstanceOf<Syringe.Web.Models.CapturedVariableItem>(viewResult.Model);
        }

        [Test]
        public void AddHeaderItem_should_return_correct_view()
        {
            // given + when
            var viewResult = testCaseController.AddHeaderItem() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/HeaderItem", viewResult.ViewName);
            Assert.IsInstanceOf<Syringe.Web.Models.HeaderItem>(viewResult.Model);
        }
    }
}
