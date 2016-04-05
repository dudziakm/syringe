using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
		private Process _serviceProcess;
		private static readonly string SERVICE_URL = "http://localhost:1337";
		private static readonly string MONGODB_DATABASE_NAME = "Syringe-Tests";
		private static readonly string BRANCH_NAME = "master";
		private string _branchDirPath;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			string xmlFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Integration");
			_branchDirPath = Path.Combine(xmlFilesPath, BRANCH_NAME);

			if (!Directory.Exists(_branchDirPath))
				Directory.CreateDirectory(_branchDirPath);

			StartServiceProcess(xmlFilesPath);
		}

		private void StartServiceProcess(string xmlFilesPath)
		{
			string buildFolder = "debug";
#if !DEBUG
			buildFolder = "release";
#endif

			string servicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Syringe.Service", "bin", buildFolder, "Syringe.Service.exe");
			servicePath = new DirectoryInfo(servicePath).FullName; // resolves the ..

			string args = $"-bindingUrl={SERVICE_URL} -mongoDbDatabaseName={MONGODB_DATABASE_NAME} -testFilesBaseDirectory={xmlFilesPath}";

			var info = new ProcessStartInfo(servicePath, args);
			info.RedirectStandardError = true;
			info.UseShellExecute = false;

			_serviceProcess = new Process();
			_serviceProcess.StartInfo = info;

			try
			{
				_serviceProcess.Start();
			}
			catch(Exception)
			{
				Console.WriteLine(_serviceProcess.StandardError.ReadToEnd());
				Assert.Fail();
			}

			Console.WriteLine("Launched {0} {1}", servicePath, args);
			Console.WriteLine("(id: {0})", _serviceProcess.Id);

			// Give the service a few seconds to startup
			Thread.Sleep(TimeSpan.FromSeconds(2));
		}

		[SetUp]
		public void Setup()
		{
			Console.WriteLine("Wiping MongoDB database {0}", MONGODB_DATABASE_NAME);

			var repository = new TestFileResultRepository(new MongoDbConfiguration(new JsonConfiguration()) { DatabaseName = MONGODB_DATABASE_NAME });
			repository.Wipe();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			if (_serviceProcess != null)
				_serviceProcess.Kill();
		}

		[Test]
		public void CreateTestFile_should_write_file()
		{
			// given
			string filename = GetXmlFilename();
			TestsClient client = CreateTestsClient();

			// when
			bool success = client.CreateTestFile(new TestFile() { Filename = filename}, BRANCH_NAME);

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
			client.CreateTestFile(new TestFile() { Filename = filename }, BRANCH_NAME);

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
			bool success = client.CreateTest(test, BRANCH_NAME);

			// then
			string fullPath = GetFullPath(filename);

			Assert.True(success);
			Assert.True(File.Exists(fullPath));
			Assert.That(new FileInfo(fullPath).Length, Is.GreaterThan(0));
			Assert.That(File.ReadAllText(fullPath), Is.StringContaining(@"errormessage=""my error message"""));
		}

		private string GetXmlFilename()
		{
			return $"{DateTime.Now.Ticks}.xml";
		}

		private string GetFullPath(string filename)
		{
			return Path.Combine(_branchDirPath, filename);
		}

		private static TestsClient CreateTestsClient()
		{
			var client = new TestsClient(SERVICE_URL);
			return client;
		}
	}
}
