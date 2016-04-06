using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Repositories.MongoDB;
using Syringe.Core.Tests;

namespace Syringe.Tests.Integration.ClientAndService
{
	[TestFixture]
	public class TestsClientTests
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			ServiceConfig.StartSelfHostedOwin();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			ServiceConfig.OwinServer.Dispose();
		}

		[SetUp]
		public void Setup()
		{
			Console.WriteLine("Wiping MongoDB database {0}", ServiceConfig.MongodbDatabaseName);
			var repository = new TestFileResultRepository(new MongoDbConfiguration(new JsonConfiguration()) { DatabaseName = ServiceConfig.MongodbDatabaseName });
			repository.Wipe();

			ServiceConfig.RecreateXmlDirectory();
		}

		private string GetXmlFilename()
		{
			return $"{DateTime.Now.Ticks}.xml";
		}

		private string GetFullPath(string filename)
		{
			return Path.Combine(ServiceConfig.XmlDirectoryPath, filename);
		}

		private TestsClient CreateTestsClient()
		{
			var client = new TestsClient(ServiceConfig.BaseUrl);
			return client;
		}

		private TestFile CreateTestFileAndTest(TestsClient client)
		{
			string filename = GetXmlFilename();
			var test1 = new Test()
			{
				Filename = filename,
				Assertions = new List<Assertion>(),
				AvailableVariables = new List<Variable>(),
				CapturedVariables = new List<CapturedVariable>(),
				ErrorMessage = "my error message 2",
				Headers = new List<HeaderItem>(),
				ShortDescription = "short desc 1",
				LongDescription = "long desc 1",
				Method = "POST",
				Url = "url 1"
			};

			var test2 = new Test()
			{
				Filename = filename,
				Assertions = new List<Assertion>(),
				AvailableVariables = new List<Variable>(),
				CapturedVariables = new List<CapturedVariable>(),
				ErrorMessage = "my error message 2",
				Headers = new List<HeaderItem>(),
				ShortDescription = "short desc 2",
				LongDescription = "long desc 2",
				Method = "POST",
				Url = "url 2"
			};

			var testFile = new TestFile() { Filename = filename };
			client.CreateTestFile(testFile, ServiceConfig.BranchName);
			client.CreateTest(test1, ServiceConfig.BranchName);
			client.CreateTest(test2, ServiceConfig.BranchName);

			var tests = new List<Test>()
			{
				test1,
				test2
			};
			testFile.Tests = tests;

			return testFile;
		}

		[Test]
		public void ListFilesForBranch_should_list_all_files()
		{
			// given
			string testFilepath1 = GetFullPath(GetXmlFilename());
			File.WriteAllText(testFilepath1, @"<?xml version=""1.0"" encoding=""utf-8"" ?><tests/>");

			string testFilepath2 = GetFullPath(GetXmlFilename());
			File.WriteAllText(testFilepath2, @"<?xml version=""1.0"" encoding=""utf-8"" ?><tests/>");

			TestsClient client = CreateTestsClient();

			// when
			IEnumerable<string> files = client.ListFilesForBranch(ServiceConfig.BranchName);

			// then
			Assert.That(files, Is.Not.Null);
			Assert.That(files.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetTest_should_return_expected_test_using_position()
		{
			// given
			int testIndex = 1;
			TestsClient client = CreateTestsClient();
			TestFile testFile = CreateTestFileAndTest(client);
			Test expectedTest = testFile.Tests.ToList()[testIndex];
			
			// when
			Test actualTest = client.GetTest(testFile.Filename, ServiceConfig.BranchName, testIndex);

			// then
			Assert.That(actualTest, Is.Not.Null);
			Assert.That(actualTest.Filename, Is.EqualTo(expectedTest.Filename));
			Assert.That(actualTest.ErrorMessage, Is.EqualTo(expectedTest.ErrorMessage));
			Assert.That(actualTest.ShortDescription, Is.EqualTo(expectedTest.ShortDescription));
		}

		[Test]
		public void GetTestFile_should_return_expected_testfile()
		{
			// given
			TestsClient client = CreateTestsClient();
			TestFile testFile = CreateTestFileAndTest(client);

			// when
			TestFile actualTestFile = client.GetTestFile(testFile.Filename, ServiceConfig.BranchName);

			// then
			Assert.That(actualTestFile, Is.Not.Null);
			Assert.That(actualTestFile.Filename, Is.EqualTo(testFile.Filename));
			Assert.That(actualTestFile.Tests.Count(), Is.EqualTo(2));
		}

		[Test]
		public void GetXml_should_return_expected_source()
		{
			// given
			TestsClient client = CreateTestsClient();
			TestFile testFile = CreateTestFileAndTest(client);

			// when
			string xml = client.GetXml(testFile.Filename, ServiceConfig.BranchName);

			// then
			Assert.That(xml, Is.Not.Null);
			Assert.That(xml, Is.StringContaining(@"<?xml version=""1.0"""));
			Assert.That(xml, Is.StringContaining("<tests"));
			Assert.That(xml, Is.StringContaining("<test"));
		}

		[Test]
		public void EditTest_should_save_changes_to_test()
		{
			// given
			TestsClient client = CreateTestsClient();
			TestFile testFile = CreateTestFileAndTest(client);
			Test expectedTest = testFile.Tests.FirstOrDefault();
			expectedTest.ShortDescription = "new description";

			// when
			bool success = client.EditTest(expectedTest, ServiceConfig.BranchName);

			// then
			Test actualTest = client.GetTest(testFile.Filename, ServiceConfig.BranchName, 0);
			
			Assert.True(success);
			Assert.That(actualTest.ShortDescription, Is.StringContaining("new description"));
		}

		[Test]
		public void CreateTest_should_create_test_for_existing_file()
		{
			// given
			string filename = GetXmlFilename();
			TestsClient client = CreateTestsClient();
			client.CreateTestFile(new TestFile() { Filename = filename }, ServiceConfig.BranchName);

			var test = new Test()
			{
				Filename = filename,
				Assertions = new List<Assertion>(),
				AvailableVariables = new List<Variable>(),
				CapturedVariables = new List<CapturedVariable>(),
				ErrorMessage = "my error message",
				Headers = new List<HeaderItem>(),
				LongDescription = "desc",
				Method = "POST",
				Url = "url"
			};

			// when
			bool success = client.CreateTest(test, ServiceConfig.BranchName);

			// then
			string fullPath = GetFullPath(filename);

			Assert.True(success);
			Assert.True(File.Exists(fullPath));
			Assert.That(new FileInfo(fullPath).Length, Is.GreaterThan(0));
			Assert.That(File.ReadAllText(fullPath), Is.StringContaining(@"errormessage=""my error message"""));
		}

		[Test]
		public void DeleteTest_should_save_changes_to_test()
		{
			// given
			TestsClient client = CreateTestsClient();
			TestFile expectedTestFile = CreateTestFileAndTest(client);

			// when
			bool success = client.DeleteTest(0, expectedTestFile.Filename, ServiceConfig.BranchName);

			// then
			TestFile actualTestFile = client.GetTestFile(expectedTestFile.Filename, ServiceConfig.BranchName);

			Assert.True(success);
			Assert.That(actualTestFile.Tests.Count(), Is.EqualTo(1));
		}

		[Test]
		public void CreateTestFile_should_write_file()
		{
			// given
			string filename = GetXmlFilename();
			TestsClient client = CreateTestsClient();

			// when
			bool success = client.CreateTestFile(new TestFile() { Filename = filename }, ServiceConfig.BranchName);

			// then
			string fullPath = GetFullPath(filename);

			Assert.True(success);
			Assert.True(File.Exists(fullPath));
			Assert.That(new FileInfo(fullPath).Length, Is.GreaterThan(0));
		}
	}
}
