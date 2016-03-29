using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Helpers;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.Tests.Results;
using Syringe.Web.Controllers;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class ResultsControllerTests
    {
        private Mock<ITasksService> _tasksServiceMock;
        private Mock<IUrlHelper> _urlHelperMock;
        private ResultsController _resultsController;
        private Guid _id;

        [SetUp]
        public void Setup()
        {
            _tasksServiceMock = new Mock<ITasksService>();
            _urlHelperMock = new Mock<IUrlHelper>();
            _id = Guid.NewGuid();
            _tasksServiceMock.Setup(x => x.GetRunningTaskDetails(It.IsAny<int>())).Returns((new TaskDetails { Results = new List<TestResult> { new TestResult { ActualUrl = "syringe.com", Id = _id } } }));
            _resultsController = new ResultsController(_tasksServiceMock.Object, _urlHelperMock.Object);
        }

        [Test]
        public void Html_should_return_404_not_found_if_case_doesnt_exist()
        {
            // given
            _tasksServiceMock.Setup(x => x.GetRunningTaskDetails(It.IsAny<int>())).Returns((new TaskDetails { Results = new List<TestResult>() }));
            
            // when
            var actionResult = _resultsController.Html(It.IsAny<int>(), It.IsAny<Guid>()) as HttpStatusCodeResult;

            // then
            Assert.AreEqual((int)HttpStatusCode.NotFound, actionResult.StatusCode);
            Assert.AreEqual("Could not locate the specified case.", actionResult.StatusDescription);
        }

        [Test]
        public void Html_should_return_correct_model()
        {
            // given + when
            var actionResult = _resultsController.Html(It.IsAny<int>(), _id) as ViewResult;

            // then
            Assert.IsInstanceOf<ResultsViewModel>(actionResult.Model);
            _tasksServiceMock.Verify(x=>x.GetRunningTaskDetails(It.IsAny<int>()),Times.Once);
        }

        [Test]
        public void Raw_should_return_404_not_found_if_case_doesnt_exist()
        {
            // given
            _tasksServiceMock.Setup(x => x.GetRunningTaskDetails(It.IsAny<int>())).Returns((new TaskDetails { Results = new List<TestResult>() }));

            // when
            var actionResult = _resultsController.Raw(It.IsAny<int>(), It.IsAny<Guid>()) as HttpStatusCodeResult;

            // then
            Assert.AreEqual((int)HttpStatusCode.NotFound, actionResult.StatusCode);
            Assert.AreEqual("Could not locate the specified case.", actionResult.StatusDescription);
        }

        [Test]
        public void Raw_should_return_correct_model()
        {
            // given + when
            var actionResult = _resultsController.Raw(It.IsAny<int>(), _id) as ViewResult;

            // then
            Assert.IsInstanceOf<ResultsViewModel>(actionResult.Model);
            _tasksServiceMock.Verify(x => x.GetRunningTaskDetails(It.IsAny<int>()), Times.Once);
        }
    }
}
