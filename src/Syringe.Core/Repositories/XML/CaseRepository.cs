using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Syringe.Core.IO;
using Syringe.Core.TestCases;
using Syringe.Core.Xml.Reader;
using Syringe.Core.Xml.Writer;

namespace Syringe.Core.Repositories.XML
{
    public class CaseRepository : ICaseRepository
    {
        private readonly ITestCaseReader _testCaseReader;
        private readonly ITestCaseWriter _testCaseWriter;
        private readonly IFileHandler _fileHandler;

        public CaseRepository(ITestCaseReader testCaseReader, ITestCaseWriter testCaseWriter, IFileHandler fileHandler)
        {
            _testCaseReader = testCaseReader;
            _testCaseWriter = testCaseWriter;
            _fileHandler = fileHandler;
        }

        public Case GetTestCase(string filename, string teamName, int index)
        {
            string fullPath = _fileHandler.GetFileFullPath(teamName, filename);
            string xml = _fileHandler.ReadAllText(fullPath);

            using (var stringReader = new StringReader(xml))
            {
                CaseCollection collection = _testCaseReader.Read(stringReader);
                Case testCase = collection.TestCases.ToList()[index];

                if (testCase == null)
                {
                    throw new NullReferenceException("Could not find specified Test Case:" + index);
                }

                testCase.ParentFilename = filename;

                return testCase;
            }
        }

        public bool CreateTestCase(Case testCase, string teamName)
        {
            if (testCase == null)
            {
                throw new ArgumentNullException("testCase");
            }

            string fullPath = _fileHandler.GetFileFullPath(teamName, testCase.ParentFilename);
            string xml = _fileHandler.ReadAllText(fullPath);

            CaseCollection collection;

            using (var stringReader = new StringReader(xml))
            {
                collection = _testCaseReader.Read(stringReader);
                collection.TestCases = collection.TestCases.Concat(new[] { testCase });
            }

            string contents = _testCaseWriter.Write(collection);

            return _fileHandler.WriteAllText(fullPath, contents);
        }

        public bool SaveTestCase(Case testCase, string teamName)
        {
            if (testCase == null)
            {
                throw new ArgumentNullException("testCase");
            }

            string fullPath = _fileHandler.GetFileFullPath(teamName, testCase.ParentFilename);
            string xml = _fileHandler.ReadAllText(fullPath);

            CaseCollection collection;

            using (var stringReader = new StringReader(xml))
            {
                collection = _testCaseReader.Read(stringReader);

                Case item = collection.TestCases.ToList()[testCase.Position];

                item.Position = testCase.Position;
                item.ShortDescription = testCase.ShortDescription;
                item.ErrorMessage = testCase.ErrorMessage;
                item.Headers = testCase.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList();
                item.LongDescription = testCase.LongDescription;
                item.Method = testCase.Method;
                item.ParentFilename = testCase.ParentFilename;
                item.ParseResponses = testCase.ParseResponses;
                item.PostBody = testCase.PostBody;
                item.VerifyPositives = testCase.VerifyPositives;
                item.VerifyNegatives = testCase.VerifyNegatives;
                item.ShortDescription = testCase.ShortDescription;
                item.Url = testCase.Url;
                item.PostType = testCase.PostType;
                item.VerifyResponseCode = testCase.VerifyResponseCode;
            }

            string contents = _testCaseWriter.Write(collection);

            return _fileHandler.WriteAllText(fullPath, contents);
        }

        public bool DeleteTestCase(int index, string fileName, string teamName)
        {
            string fullPath = _fileHandler.GetFileFullPath(teamName, fileName);
            string xml = _fileHandler.ReadAllText(fullPath);

            CaseCollection collection;

            using (var stringReader = new StringReader(xml))
            {
                collection = _testCaseReader.Read(stringReader);

                var tests = collection.TestCases.ToList();

                tests.RemoveAt(index);

                collection.TestCases = tests;
            }

            string contents = _testCaseWriter.Write(collection);

            return _fileHandler.WriteAllText(fullPath, contents);
        }

        public bool CreateTestFile(CaseCollection caseCollection, string teamName)
        {
            caseCollection.Filename = _fileHandler.CreateFilename(caseCollection.Filename);

            string filePath = _fileHandler.CreateFileFullPath(teamName, caseCollection.Filename);
            bool fileExists = _fileHandler.FileExists(filePath);

            if (fileExists)
            {
                throw new IOException("File already exists");
            }

            string contents = _testCaseWriter.Write(caseCollection);

            return _fileHandler.WriteAllText(filePath, contents);
        }

        public bool UpdateTestFile(CaseCollection caseCollection, string teamName)
        {
            string fileFullPath = _fileHandler.GetFileFullPath(teamName, caseCollection.Filename);
            string xml = _fileHandler.ReadAllText(fileFullPath);

            using (var stringReader = new StringReader(xml))
            {
                CaseCollection collection = _testCaseReader.Read(stringReader);

                collection.Variables = caseCollection.Variables;
                collection.Repeat = caseCollection.Repeat;

                string contents = _testCaseWriter.Write(collection);
                return _fileHandler.WriteAllText(fileFullPath, contents);
            }
        }

        public CaseCollection GetTestCaseCollection(string filename, string teamName)
        {
            string fullPath = _fileHandler.GetFileFullPath(teamName, filename);
            string xml = _fileHandler.ReadAllText(fullPath);

            using (var stringReader = new StringReader(xml))
            {
                return _testCaseReader.Read(stringReader);
            }
        }

        public string GetXmlTestCaseCollection(string filename, string teamName)
        {
            var fullPath = _fileHandler.GetFileFullPath(teamName, filename);
            return _fileHandler.ReadAllText(fullPath);
        }

        public IEnumerable<string> ListCasesForTeam(string teamName)
        {
            string fullPath = _fileHandler.GetTeamDirectoryFullPath(teamName);
            return _fileHandler.GetFileNames(fullPath);
        }
    }
}