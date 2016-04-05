using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Syringe.Client;
using Syringe.Core.Configuration;
using Syringe.Core.Repositories.MongoDB;
using Syringe.Core.Tests;

namespace Syringe.Tests.Integration.Service
{
	[TestFixture]
	public class TestsClientTests
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			ServiceConfig.CreateXmlDirectory();
			ServiceConfig.StartSelfHostedOwin();
		}
	
		[SetUp]
		public void Setup()
		{
			Console.WriteLine("Wiping MongoDB database {0}", ServiceConfig.MongodbDatabaseName);

			var repository = new TestFileResultRepository(new MongoDbConfiguration(new JsonConfiguration()) { DatabaseName = ServiceConfig.MongodbDatabaseName });
			repository.Wipe();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			ServiceConfig.OwinServer.Dispose();
		}

		private string GetXmlFilename()
		{
			return $"{DateTime.Now.Ticks}.xml";
		}

		private string GetFullPath(string filename)
		{
			return Path.Combine(ServiceConfig.XmlDirectoryPath, filename);
		}

		private static TestsClient CreateTestsClient()
		{
			var client = new TestsClient(ServiceConfig.BaseUrl);
			return client;
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
	}
}
