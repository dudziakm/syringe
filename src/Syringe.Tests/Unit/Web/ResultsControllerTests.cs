using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Helpers;
using Syringe.Core.Results;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Web.Controllers;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class ResultsControllerTests
    {
        private Mock<ITasksService> tasksServiceMock;
        private Mock<IUrlHelper> urlHelperMock;
        private ResultsController resultsController;
        private Guid Id;

        [SetUp]
        public void Setup()
        {
            tasksServiceMock = new Mock<ITasksService>();
            urlHelperMock = new Mock<IUrlHelper>();

            Id = Guid.NewGuid();

            tasksServiceMock.Setup(x => x.GetRunningTaskDetails(It.IsAny<int>())).Returns((new TaskDetails { Results = new List<TestCaseResult> { new TestCaseResult { ActualUrl = "syringe.com", Id = Id } } }));


            resultsController = new ResultsController(tasksServiceMock.Object, urlHelperMock.Object);
        }

        [Test]
        public void Html_should_return_404_not_found_if_case_doesnt_exist()
        {
            // given
            tasksServiceMock.Setup(x => x.GetRunningTaskDetails(It.IsAny<int>())).Returns((new TaskDetails { Results = new List<TestCaseResult>() }));
            
            // when
            var actionResult = resultsController.Html(It.IsAny<int>(), It.IsAny<Guid>()) as HttpStatusCodeResult;

            // then
            Assert.AreEqual((int)HttpStatusCode.NotFound, actionResult.StatusCode);
            Assert.AreEqual("Could not locate the specified case.", actionResult.StatusDescription);
        }

        [Test]
        public void Html_should_return_correct_model()
        {
            // given + when
            var actionResult = resultsController.Html(It.IsAny<int>(), Id) as ViewResult;

            // then
            Assert.IsInstanceOf<ResultsViewModel>(actionResult.Model);
            tasksServiceMock.Verify(x=>x.GetRunningTaskDetails(It.IsAny<int>()),Times.Once);
        }

        [Test]
        public void Raw_should_return_404_not_found_if_case_doesnt_exist()
        {
            // given
            tasksServiceMock.Setup(x => x.GetRunningTaskDetails(It.IsAny<int>())).Returns((new TaskDetails { Results = new List<TestCaseResult>() }));

            // when
            var actionResult = resultsController.Raw(It.IsAny<int>(), It.IsAny<Guid>()) as HttpStatusCodeResult;

            // then
            Assert.AreEqual((int)HttpStatusCode.NotFound, actionResult.StatusCode);
            Assert.AreEqual("Could not locate the specified case.", actionResult.StatusDescription);
        }

        [Test]
        public void Raw_should_return_correct_model()
        {
            // given + when
            var actionResult = resultsController.Raw(It.IsAny<int>(), Id) as ViewResult;

            // then
            Assert.IsInstanceOf<ResultsViewModel>(actionResult.Model);
            tasksServiceMock.Verify(x => x.GetRunningTaskDetails(It.IsAny<int>()), Times.Once);
        }
    }
}
