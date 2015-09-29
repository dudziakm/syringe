using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.TestCases;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
	[TestFixture]
	public class RunViewModelTests
	{
		private static RunViewModel GivenARunViewModel(
			ITasksService tasksService = null,
			ICaseService caseService = null,
            IApplicationConfiguration applicationConfiguration = null)
		{
			return new RunViewModel(
				tasksService ?? Mock.Of<ITasksService>(),
				caseService ?? Mock.Of<ICaseService>(),
                applicationConfiguration ?? Mock.Of<IApplicationConfiguration>());
		}

		[TestFixture]
		public class when_running
		{
			[Test]
			public void should_set_filename_to_specified_file()
			{
				const string fileName = "Some file";
				var caseService =
					Mock.Of<ICaseService>(
						s =>
							s.GetTestCaseCollection(It.IsAny<string>(), It.IsAny<string>()) == Mock.Of<CaseCollection>());

				var viewModel = GivenARunViewModel(caseService: caseService);

				viewModel.Run(Mock.Of<IUserContext>(), fileName);

				Assert.That(viewModel.FileName, Is.EqualTo(fileName));
			}

			[Test]
			public void should_populate_test_cases_using_specified_filename()
			{
				const string fileName = "Some file";
				const string teamName = "My team";

				var cases =
					Mock.Of<CaseCollection>(
						c =>
							c.TestCases ==
							new[]
							{
								new Case {Id = 1, ShortDescription = "Desc1"},
								new Case {Id = 2, ShortDescription = "Desc2"}
							});

				var caseService =
					Mock.Of<ICaseService>(
						s =>
							s.GetTestCaseCollection(fileName, teamName) == cases);

				var viewModel = GivenARunViewModel(caseService: caseService);

				viewModel.Run(Mock.Of<IUserContext>(c => c.TeamName == teamName), fileName);

				Assert.That(viewModel.TestCases, Is.Not.Null);
				Assert.That(viewModel.TestCases.Select(c => new { c.Id, c.Description }), Is.EquivalentTo(new[]
				{
					new { Id = 1, Description = "Desc1" },
					new { Id = 2, Description = "Desc2" }
				}));
			}

			[Test]
			public void should_start_task_using_current_user_context_and_file_name()
			{
				const string fileName = "MyFile";
				const string userName = "Me";
				const string teamName = "My team";

				var caseService =
					Mock.Of<ICaseService>(
						s =>
							s.GetTestCaseCollection(It.IsAny<string>(), It.IsAny<string>()) == Mock.Of<CaseCollection>());

				var tasksService = new Mock<ITasksService>();

				var viewModel = GivenARunViewModel(caseService: caseService, tasksService: tasksService.Object);

				viewModel.Run(Mock.Of<IUserContext>(c => c.TeamName == teamName && c.Username == userName), fileName);

				tasksService.Verify(
					s =>
						s.Start(
							It.Is<TaskRequest>(
								r => r.TeamName == teamName && r.Filename == fileName && r.Username == userName)),
					"Should have requested for the correct task to start.");
			}

			[Test]
			public void should_set_current_task_id_to_running_task_id()
			{
				const int taskId = 121;

				var caseService =
					Mock.Of<ICaseService>(
						s =>
							s.GetTestCaseCollection(It.IsAny<string>(), It.IsAny<string>()) == Mock.Of<CaseCollection>());

				var tasksService = Mock.Of<ITasksService>(s => s.Start(It.IsAny<TaskRequest>()) == taskId);

				var viewModel = GivenARunViewModel(caseService: caseService, tasksService: tasksService);

				viewModel.Run(Mock.Of<IUserContext>(), "My test file");

				Assert.That(viewModel.CurrentTaskId, Is.EqualTo(taskId));
			}
		}
	}
}
