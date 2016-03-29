using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Syringe.Core.Services;
using Syringe.Core.Security;
using Syringe.Core.Tasks;
using Syringe.Core.TestCases;
using Syringe.Web.Controllers;

namespace Syringe.Tests.Unit.Web
{
    [TestFixture]
    public class JsonControllerTests
    {
        private Mock<ITasksService> _tasksClient;
        private Mock<ICaseService> _casesClient;
        private Mock<IUserContext> _userContext;
        private JsonController jsonController;
        [SetUp]
        public void Setup()
        {
            _tasksClient = new Mock<ITasksService>();
            _casesClient = new Mock<ICaseService>();
            _userContext = new Mock<IUserContext>();

            _tasksClient.Setup(x => x.Start(It.IsAny<TaskRequest>())).Returns(10);
            _tasksClient.Setup(x => x.GetRunningTaskDetails(It.IsAny<int>())).Returns(new TaskDetails());
            _casesClient.Setup(x => x.GetTestCaseCollection(It.IsAny<string>(),It.IsAny<string>())).Returns(new CaseCollection());
            jsonController = new JsonController(_tasksClient.Object, _casesClient.Object, _userContext.Object);
        }

        [Test]
        public void Run_should_return_correct_json()
        {
            // given + when
            var actionResult = jsonController.Run(It.IsAny<string>()) as JsonResult;

            // then
            _tasksClient.Verify(x => x.Start(It.IsAny<TaskRequest>()), Times.Once);
            Assert.AreEqual("{ taskId = 10 }", actionResult.Data.ToString());
        }

        [Test]
        public void GetProgress_should_return_correct_json()
        {
            // given + when
            var actionResult = jsonController.GetProgress(It.IsAny<int>()) as ContentResult;

            // then
            _tasksClient.Verify(x => x.GetRunningTaskDetails(It.IsAny<int>()), Times.Once);
            Assert.AreEqual("{\"TaskId\":0,\"Filename\":null,\"Username\":null,\"TeamName\":null,\"Status\":null,\"CurrentIndex\":0,\"TotalCases\":0,\"Results\":[],\"Errors\":null}", actionResult.Content);
        }

        [Test]
        public void GetCases_should_return_correct_json()
        {
            // given + when
            var actionResult = jsonController.GetCases(It.IsAny<string>()) as ContentResult;

            // then
            _casesClient.Verify(x => x.GetTestCaseCollection(It.IsAny<string>(),It.IsAny<string>()), Times.Once);
            Assert.AreEqual("{\"Repeat\":0,\"TestCases\":[],\"Filename\":null,\"Variables\":[]}", actionResult.Content);
        }
    }
}
