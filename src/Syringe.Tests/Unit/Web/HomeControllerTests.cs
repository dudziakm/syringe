using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Canary;
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
        private Mock<Func<ICanaryService>> _canaryClientFactory;
        private HomeController homeController;
        private Mock<ICanaryService> canaryClientService;
        private Mock<IRunViewModel> runViewModelMockService;
        [SetUp]
        public void Setup()
        {
            _casesClient = new Mock<ICaseService>();
            _userContext = new Mock<IUserContext>();

            _canaryClientFactory = new Mock<Func<ICanaryService>>();
            canaryClientService = new Mock<ICanaryService>();
            canaryClientService.Setup(x => x.Check()).Returns(new CanaryResult { Success = true });
            _canaryClientFactory.Setup(x => x()).Returns(canaryClientService.Object);


            _runViewModelFactory = new Mock<Func<IRunViewModel>>();
            runViewModelMockService = new Mock<IRunViewModel>();
            runViewModelMockService.Setup(x => x.Run(It.IsAny<UserContext>(), It.IsAny<string>()));
            _runViewModelFactory.Setup(x => x()).Returns(runViewModelMockService.Object);

            _casesClient.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(new TestCaseSession());

            _casesClient.Setup(x => x.GetSummaries()).Returns(new List<SessionInfo>());
            _casesClient.Setup(x => x.GetSummariesForToday()).Returns(new List<SessionInfo>());

            homeController = new HomeController(_casesClient.Object, _userContext.Object, _runViewModelFactory.Object,
                _canaryClientFactory.Object);
        }

        [Test]
        public void AllResults_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = homeController.AllResults() as ViewResult;

            // then
            _casesClient.Verify(x => x.GetSummaries(), Times.Once);
            Assert.AreEqual("AllResults", viewResult.ViewName);
            Assert.IsInstanceOf<IEnumerable<SessionInfo>>(viewResult.Model);
        }

        [Test]
        public void TodaysResults_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = homeController.TodaysResults() as ViewResult;

            // then
            _casesClient.Verify(x => x.GetSummariesForToday(), Times.Once);
            Assert.AreEqual("AllResults", viewResult.ViewName);
            Assert.IsInstanceOf<IEnumerable<SessionInfo>>(viewResult.Model);
        }


        [Test]
        public void ViewResult_should_return_correct_view_and_model()
        {
            // given + when
            var viewResult = homeController.ViewResult(It.IsAny<Guid>()) as ViewResult;

            // then
            _casesClient.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Once);
            Assert.AreEqual("ViewResult", viewResult.ViewName);
            Assert.IsInstanceOf<TestCaseSession>(viewResult.Model);
        }

        [Test]
        public async void DeleteResult_should_call_delete_methods_and_redirect_to_correct_action()
        {
            // given + when
            var redirectToRouteResult = await homeController.DeleteResult(It.IsAny<Guid>()) as RedirectToRouteResult;

            // then
            _casesClient.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Once);
            _casesClient.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Once);
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

        [Test]
        public void Index_should_throw_InvalidOperationException_if_service_is_null()
        {
            // given + when
            canaryClientService.Setup(x => x.Check()).Returns((CanaryResult)null);

            // then
            Assert.Throws<InvalidOperationException>(() => homeController.Index(It.IsAny<int>(), It.IsAny<int>()), "Unable to connect to the REST api service.Is the service started ? Check it at http://localhost:1981/");
            _canaryClientFactory.Verify(x => x(), Times.Once);
        }

        [Test]
        public void Index_should_throw_InvalidOperationException_if_service_is_not_running()
        {
            // given + when
            canaryClientService.Setup(x => x.Check()).Returns(new CanaryResult { Success = false });

            // then
            Assert.Throws<InvalidOperationException>(() => homeController.Index(It.IsAny<int>(), It.IsAny<int>()), "Unable to connect to the REST api service.Is the service started ? Check it at http://localhost:1981/");
            _canaryClientFactory.Verify(x => x(), Times.Once);

        }

        [Test]
        public void Index_should_call_run_method_and_return_correct_model()
        {
            // given + when
            var viewResult = homeController.Index(It.IsAny<int>(), It.IsAny<int>()) as ViewResult;

            // then
            _casesClient.Verify(x => x.ListFilesForTeam(It.IsAny<string>()), Times.Once);
            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.IsInstanceOf<IndexViewModel>(viewResult.Model);
        }
    }
}
