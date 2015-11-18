using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.TestCases;
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
        private Mock<ITestCaseMapper> ITestCaseMapperMock;

        [SetUp]
        public void Setup()
        {
            ICaseServiceMock = new Mock<ICaseService>();
            IUserContextMock = new Mock<IUserContext>();
            ITestCaseMapperMock = new Mock<ITestCaseMapper>();

            IUserContextMock.Setup(x => x.TeamName).Returns("Team");
            ITestCaseMapperMock.Setup(x => x.BuildTestCases(It.IsAny<IEnumerable<Case>>()));
            ITestCaseMapperMock.Setup(x => x.BuildViewModel(It.IsAny<Case>())).Returns(new TestCaseViewModel());
            ICaseServiceMock.Setup(x => x.GetTestCaseCollection(It.IsAny<string>(), IUserContextMock.Object.TeamName)).Returns(new CaseCollection());
            ICaseServiceMock.Setup(x => x.GetTestCase(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()));

            testCaseController = new TestCaseController(ICaseServiceMock.Object, IUserContextMock.Object, ITestCaseMapperMock.Object);
        }

        [Test]
        public void View_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = testCaseController.View(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            // then
            ICaseServiceMock.Verify(x => x.GetTestCaseCollection(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            ITestCaseMapperMock.Verify(x => x.BuildTestCases(It.IsAny<IEnumerable<Case>>()), Times.Once);
            Assert.AreEqual("View", viewResult.ViewName);
            Assert.IsInstanceOf<TestFileViewModel>(viewResult.Model);
        }

        [Test]
        public void Edit_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = testCaseController.Edit(It.IsAny<string>(), It.IsAny<Guid>()) as ViewResult;

            // then
            ICaseServiceMock.Verify(x => x.GetTestCase(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<Guid>()), Times.Once);
            ITestCaseMapperMock.Verify(x => x.BuildViewModel(It.IsAny<Case>()), Times.Once);
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<TestCaseViewModel>(viewResult.Model);
        }

        [Test]
        public void Add_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = testCaseController.Add(It.IsAny<string>()) as ViewResult;

            // then
            Assert.AreEqual("Edit", viewResult.ViewName);
            Assert.IsInstanceOf<TestCaseViewModel>(viewResult.Model);
        }

        [Test]
        public void AddVerification_should_return_correct_view()
        {
            // given + when
            var viewResult = testCaseController.AddVerification() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/VerificationItemModel", viewResult.ViewName);
            Assert.IsInstanceOf<VerificationItemModel>(viewResult.Model);
        }

        [Test]
        public void AddParseResponseItem_should_return_correct_view()
        {
            // given + when
            var viewResult = testCaseController.AddParseResponseItem() as PartialViewResult;

            // then
            Assert.AreEqual("EditorTemplates/ParseResponseItem", viewResult.ViewName);
            Assert.IsInstanceOf<Syringe.Web.Models.ParseResponseItem>(viewResult.Model);
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
