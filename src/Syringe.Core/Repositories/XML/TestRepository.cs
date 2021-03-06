﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Syringe.Core.IO;
using Syringe.Core.Tests;
using Syringe.Core.Xml.Reader;
using Syringe.Core.Xml.Writer;

namespace Syringe.Core.Repositories.XML
{
    public class TestRepository : ITestRepository
    {
        private readonly ITestFileReader _testFileReader;
        private readonly ITestFileWriter _testFileWriter;
        private readonly IFileHandler _fileHandler;

        public TestRepository(ITestFileReader testFileReader, ITestFileWriter testFileWriter, IFileHandler fileHandler)
        {
            _testFileReader = testFileReader;
            _testFileWriter = testFileWriter;
            _fileHandler = fileHandler;
        }

        public Test GetTest(string filename, string branchName, int position)
        {
            string fullPath = _fileHandler.GetFileFullPath(branchName, filename);
            string xml = _fileHandler.ReadAllText(fullPath);

            using (var stringReader = new StringReader(xml))
            {
                TestFile testFile = _testFileReader.Read(stringReader);
                Test testTest = testFile.Tests.ElementAtOrDefault(position);

                if (testTest == null)
                {
                    throw new NullReferenceException("Could not find specified Test Case:" + position);
                }

                testTest.Filename = filename;

                return testTest;
            }
        }

        public bool CreateTest(Test test, string branchName)
        {
            if (test == null)
            {
                throw new ArgumentNullException("test");
            }

            string fullPath = _fileHandler.GetFileFullPath(branchName, test.Filename);
            string xml = _fileHandler.ReadAllText(fullPath);

            TestFile collection;

            using (var stringReader = new StringReader(xml))
            {
                collection = _testFileReader.Read(stringReader);

                collection.Tests = collection.Tests.Concat(new[] { test });
            }

            string contents = _testFileWriter.Write(collection);

            return _fileHandler.WriteAllText(fullPath, contents);
        }

        public bool SaveTest(Test test, string branchName)
        {
            if (test == null)
            {
                throw new ArgumentNullException("test");
            }

            string fullPath = _fileHandler.GetFileFullPath(branchName, test.Filename);
            string xml = _fileHandler.ReadAllText(fullPath);

            TestFile collection;

            using (var stringReader = new StringReader(xml))
            {
                collection = _testFileReader.Read(stringReader);

                Test item = collection.Tests.ElementAt(test.Position);

                item.ShortDescription = test.ShortDescription;
                item.ErrorMessage = test.ErrorMessage;
                item.Headers = test.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList();
                item.LongDescription = test.LongDescription;
                item.Method = test.Method;
                item.Filename = test.Filename;
                item.CapturedVariables = test.CapturedVariables;
                item.PostBody = test.PostBody;
                item.Assertions = test.Assertions;
                item.ShortDescription = test.ShortDescription;
                item.Url = test.Url;
                item.PostType = test.PostType;
                item.VerifyResponseCode = test.VerifyResponseCode;
            }

            string contents = _testFileWriter.Write(collection);

            return _fileHandler.WriteAllText(fullPath, contents);
        }

        public bool DeleteTest(int position, string fileName, string branchName)
        {
            string fullPath = _fileHandler.GetFileFullPath(branchName, fileName);
            string xml = _fileHandler.ReadAllText(fullPath);

            TestFile testFile;

            using (var stringReader = new StringReader(xml))
            {
                testFile = _testFileReader.Read(stringReader);

                Test testToDelete = testFile.Tests.ElementAtOrDefault(position);

                if (testToDelete == null)
                {
                    throw new NullReferenceException(string.Concat("could not find test case:", position));
                }

                testFile.Tests = testFile.Tests.Where(x => x != testToDelete);
            }

            string contents = _testFileWriter.Write(testFile);

            return _fileHandler.WriteAllText(fullPath, contents);
        }

        public bool CreateTestFile(TestFile testFile, string branchName)
        {
            testFile.Filename = _fileHandler.CreateFilename(testFile.Filename);

            string filePath = _fileHandler.CreateFileFullPath(branchName, testFile.Filename);
            bool fileExists = _fileHandler.FileExists(filePath);

            if (fileExists)
            {
                throw new IOException("File already exists");
            }

            string contents = _testFileWriter.Write(testFile);

            return _fileHandler.WriteAllText(filePath, contents);
        }

        public bool UpdateTestFile(TestFile testFile, string branchName)
        {
            string fileFullPath = _fileHandler.GetFileFullPath(branchName, testFile.Filename);
            string xml = _fileHandler.ReadAllText(fileFullPath);

            using (var stringReader = new StringReader(xml))
            {
                TestFile collection = _testFileReader.Read(stringReader);

                collection.Variables = testFile.Variables;
                collection.Repeat = testFile.Repeat;

                string contents = _testFileWriter.Write(collection);
                return _fileHandler.WriteAllText(fileFullPath, contents);
            }
        }

        public TestFile GetTestFile(string filename, string branchName)
        {
            string fullPath = _fileHandler.GetFileFullPath(branchName, filename);
            string xml = _fileHandler.ReadAllText(fullPath);

            using (var stringReader = new StringReader(xml))
            {
                return _testFileReader.Read(stringReader);
            }
        }

        public string GetXml(string filename, string branchName)
        {
            var fullPath = _fileHandler.GetFileFullPath(branchName, filename);
            return _fileHandler.ReadAllText(fullPath);
        }

        public IEnumerable<string> ListFilesForBranch(string branchName)
        {
            string fullPath = _fileHandler.GetBranchDirectoryFullPath(branchName);
            return _fileHandler.GetFileNames(fullPath);
        }
    }
}