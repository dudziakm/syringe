using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Syringe.Core.Configuration;
using Syringe.Core.Logging;
using Syringe.Core.Xml.Reader;
using Syringe.Core.Xml.Writer;

namespace Syringe.Core.Domain.Repositories
{
	public class CaseRepository : ICaseRepository
	{
		private readonly ITestCaseReader _testCaseReader;
		private readonly IApplicationConfiguration _appConfig;
		private readonly ITestCaseWriter _testCaseWriter;

		public CaseRepository() : this(new TestCaseReader(), new ApplicationConfig(), new TestCaseWriter()) { }

		internal CaseRepository(ITestCaseReader testCaseReader, IApplicationConfiguration appConfig, ITestCaseWriter testCaseWriter)
		{
			_testCaseReader = testCaseReader;
			_appConfig = appConfig;
			_testCaseWriter = testCaseWriter;
		}

		public Case GetTestCase(string filename, string teamName, int caseId)
		{
			string fullPath = Path.Combine(_appConfig.TestCasesBaseDirectory, teamName, filename);
			if (!File.Exists(fullPath))
				throw new FileNotFoundException("The test case cannot be found", filename);


			string xml = File.ReadAllText(fullPath);
			using (var stringReader = new StringReader(xml))
			{
				CaseCollection collection = _testCaseReader.Read(stringReader);
				Case testCase = collection.TestCases.First(x => x.Id == caseId);

				if (testCase == null)
				{
					throw new NullReferenceException("Could not find specified Test Case:" + caseId);
				}

				testCase.ParentFilename = filename;

				return testCase;
			}
		}

		public bool SaveTestCase(Case testCase, string teamName)
		{
			if (testCase == null)
			{
				throw new ArgumentNullException("testCase");
			}

			string fullPath = Path.Combine(_appConfig.TestCasesBaseDirectory, teamName, testCase.ParentFilename);
			if (!File.Exists(fullPath))
				throw new FileNotFoundException("The test case cannot be found", testCase.ParentFilename);

			CaseCollection collection;
			string xml = File.ReadAllText(fullPath);
			using (var stringReader = new StringReader(xml))
			{
				collection = _testCaseReader.Read(stringReader);

				foreach (var item in collection.TestCases.Where(x => x.Id == testCase.Id))
				{
					item.Id = testCase.Id;
					item.ShortDescription = testCase.ShortDescription;
					item.ErrorMessage = testCase.ErrorMessage;
					item.Headers = testCase.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList();
					item.LogRequest = testCase.LogRequest;
					item.LogResponse = testCase.LogResponse;
					item.LongDescription = testCase.LongDescription;
					item.Method = testCase.Method;
					item.ParentFilename = testCase.ParentFilename;
					item.ParseResponses = testCase.ParseResponses;
					item.PostBody = testCase.PostBody;
					item.VerifyPositives = testCase.VerifyPositives;
					item.VerifyNegatives = testCase.VerifyNegatives;
					item.ShortDescription = testCase.ShortDescription;
					item.Url = testCase.Url;
					item.Sleep = testCase.Sleep;
					item.PostType = testCase.PostType;
					item.VerifyResponseCode = testCase.VerifyResponseCode;
				}
			}

			string contents = _testCaseWriter.Write(collection);

			try
			{
				File.WriteAllText(fullPath, contents);
				return true;
			}
			catch (Exception exception)
			{
				//todo log error
				Log.Error(exception, exception.Message);
			}

			return false;
		}

		public CaseCollection GetTestCaseCollection(string filename, string teamName)
		{
			string fullPath = Path.Combine(_appConfig.TestCasesBaseDirectory, teamName, filename);
			string xml = File.ReadAllText(fullPath);

			using (var stringReader = new StringReader(xml))
			{
				return _testCaseReader.Read(stringReader);
			}
		}

		public IEnumerable<string> ListCasesForTeam(string teamName)
		{
			string fullPath = Path.Combine(_appConfig.TestCasesBaseDirectory, teamName);

			foreach (string file in Directory.EnumerateFiles(fullPath))
			{
				var fileInfo = new FileInfo(file);
				yield return fileInfo.Name;
			}
		}
	}
}