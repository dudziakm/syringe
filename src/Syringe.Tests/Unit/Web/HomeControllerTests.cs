using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Repositories;
using Syringe.Core.Results;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Web.Controllers;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<ICaseService> _casesClient;
        private Mock<IUserContext> _userContext;
        private Mock<Func<IRunViewModel>> _runViewModelFactory;
        private Mock<ITestCaseSessionRepository> _repository;
        private Mock<Func<ICanaryService>> _canaryClientFactory;
        private HomeController homeController;
        [SetUp]
        public void Setup()
        {
            _casesClient = new Mock<ICaseService>();
            _userContext = new Mock<IUserContext>();
            _repository = new Mock<ITestCaseSessionRepository>();
            _canaryClientFactory = new Mock<Func<ICanaryService>>();
            _runViewModelFactory = new Mock<Func<IRunViewModel>>();
            var mockService = new Mock<IRunViewModel>();
            mockService.Setup(x => x.Run(It.IsAny<UserContext>(), It.IsAny<string>()));
            _runViewModelFactory.Setup(x => x()).Returns(mockService.Object);

            _repository.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(new TestCaseSession());

            homeController = new HomeController(_casesClient.Object, _userContext.Object, _runViewModelFactory.Object,
                _canaryClientFactory.Object, _repository.Object);
        }

        [Test]
        public void AllResults_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = homeController.AllResults() as ViewResult;

            // then
            _repository.Verify(x => x.GetSummaries(), Times.Once);
            Assert.AreEqual("AllResults", viewResult.ViewName);
            Assert.IsInstanceOf<IEnumerable<SessionInfo>>(viewResult.Model);
        }

        [Test]
        public void TodaysResults_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = homeController.TodaysResults() as ViewResult;

            // then
            _repository.Verify(x => x.GetSummariesForToday(), Times.Once);
            Assert.AreEqual("AllResults", viewResult.ViewName);
            Assert.IsInstanceOf<IEnumerable<SessionInfo>>(viewResult.Model);
        }


        [Test]
        public void ViewResult_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = homeController.ViewResult(It.IsAny<Guid>()) as ViewResult;

            // then
            _repository.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Once);
            Assert.AreEqual("ViewResult", viewResult.ViewName);
            Assert.IsInstanceOf<TestCaseSession>(viewResult.Model);
        }

        [Test]
        public async void DeleteResult_should_call_delete_methods_and_redirect_to_correct_action()
        {
            // given + when
            var redirectToRouteResult = await homeController.DeleteResult(It.IsAny<Guid>()) as RedirectToRouteResult;

            // then
            _repository.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Once);
            _repository.Verify(x => x.DeleteAsync(It.IsAny<TestCaseSession>()), Times.Once);
            Assert.AreEqual("AllResults", redirectToRouteResult.RouteValues["action"]);
        }

        [Test]
        public void Run_should_call_run_method_and_return_correct_model()
        {
            // given + when
            var viewResult = homeController.Run(It.IsAny<string>()) as ViewResult;

            // then
            _runViewModelFactory.Verify(x => x(), Times.Once);
            Assert.AreEqual("Run", viewResult.ViewName);
            Assert.IsInstanceOf<IRunViewModel>(viewResult.Model);
        }
    }
}
