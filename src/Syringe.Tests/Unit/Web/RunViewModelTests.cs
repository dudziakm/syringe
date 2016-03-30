﻿using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.Tests;
using Syringe.Web;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.Web
{
	[TestFixture]
	public class RunViewModelTests
	{
		private static RunViewModel GivenARunViewModel(ITasksService tasksService = null, ITestService testService = null)
		{
			return new RunViewModel(
				tasksService ?? Mock.Of<ITasksService>(),
				testService ?? Mock.Of<ITestService>());
		}

		[TestFixture]
		public class when_running
		{
			[Test]
			public void should_set_filename_to_specified_file()
			{
				const string fileName = "Some file";
				var caseService =
					Mock.Of<ITestService>(
						s =>
							s.GetTestFile(It.IsAny<string>(), It.IsAny<string>()) == Mock.Of<TestFile>());

				var viewModel = GivenARunViewModel(testService: caseService);

				viewModel.Run(Mock.Of<IUserContext>(), fileName);

				Assert.That(viewModel.FileName, Is.EqualTo(fileName));
			}

			[Test]
			public void should_populate_test_cases_using_specified_filename()
			{
				const string fileName = "Some file";
				const string teamName = "My team";
			    var testCase1 = 1;
			    var testCase2 = 2;
				var cases =
					Mock.Of<TestFile>(
						c =>
							c.Tests ==
							new[]
							{
								new Test { Position = testCase1, ShortDescription = "Desc1"},
								new Test { Position = testCase2, ShortDescription = "Desc2"}
							});

				var caseService =
					Mock.Of<ITestService>(
						s =>
							s.GetTestFile(fileName, teamName) == cases);

				var viewModel = GivenARunViewModel(testService: caseService);

				viewModel.Run(Mock.Of<IUserContext>(c => c.DefaultBranchName == teamName), fileName);

				Assert.That(viewModel.Tests, Is.Not.Null);
				Assert.That(viewModel.Tests.Select(c => new { c.Position, c.Description }), Is.EquivalentTo(new[]
				{
					new { Position = testCase1, Description = "Desc1" },
					new { Position = testCase2, Description = "Desc2" }
				}));
			}

			[Test]
			public void should_start_task_using_current_user_context_and_file_name()
			{
				const string fileName = "MyFile";
				const string userName = "Me";
				const string teamName = "My team";

				var caseService =
					Mock.Of<ITestService>(
						s =>
							s.GetTestFile(It.IsAny<string>(), It.IsAny<string>()) == Mock.Of<TestFile>());

				var tasksService = new Mock<ITasksService>();

				var viewModel = GivenARunViewModel(testService: caseService, tasksService: tasksService.Object);

				viewModel.Run(Mock.Of<IUserContext>(c => c.DefaultBranchName == teamName && c.FullName == userName), fileName);

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
					Mock.Of<ITestService>(
						s =>
							s.GetTestFile(It.IsAny<string>(), It.IsAny<string>()) == Mock.Of<TestFile>());

				var tasksService = Mock.Of<ITasksService>(s => s.Start(It.IsAny<TaskRequest>()) == taskId);

				var viewModel = GivenARunViewModel(testService: caseService, tasksService: tasksService);

				viewModel.Run(Mock.Of<IUserContext>(), "My test file");

				Assert.That(viewModel.CurrentTaskId, Is.EqualTo(taskId));
			}
		}
	}
}
