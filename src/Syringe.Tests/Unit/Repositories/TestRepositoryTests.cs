﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.IO;
using Syringe.Core.Repositories;
using Syringe.Core.Repositories.XML;
using Syringe.Core.Tests;
using Syringe.Core.Xml.Reader;
using Syringe.Core.Xml.Writer;

namespace Syringe.Tests.Unit.Repositories
{
    [TestFixture]
    public class TestRepositoryTests
    {
        private Mock<ITestFileReader> _testFileReader;
        private Mock<ITestFileWriter> _testFileWriter;
        private Mock<IFileHandler> _fileHandler;
        private TestRepository _testRepository;

        [SetUp]
        public void Setup()
        {
            _testFileReader = new Mock<ITestFileReader>();
            _testFileWriter = new Mock<ITestFileWriter>();
            _fileHandler = new Mock<IFileHandler>();

            _fileHandler.Setup(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>())).Returns("path");
            _fileHandler.Setup(x => x.CreateFileFullPath(It.IsAny<string>(), It.IsAny<string>())).Returns("filepath.xml");
            _fileHandler.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("<xml></xml>");
            _testFileReader.Setup(x => x.Read(It.IsAny<TextReader>())).Returns(new TestFile { Filename="filepath.xml", Tests = new List<Test> { new Test() } });
            _testRepository = new TestRepository(_testFileReader.Object, _testFileWriter.Object, _fileHandler.Object);
            _fileHandler.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _fileHandler.Setup(x => x.GetFileNames(It.IsAny<string>())).Returns(new List<string> {{"test"}});
            _testFileWriter.Setup(x => x.Write(It.IsAny<TestFile>())).Returns("<xml></xml>");
        }

        [Test]
        public void GetTest_should_throw_null_reference_exception_when_position_is_invalid()
        {
            // given + when
            _testFileReader.Setup(x => x.Read(It.IsAny<TextReader>())).Returns(new TestFile());

            // then
            Assert.Throws<NullReferenceException>(() => _testRepository.GetTest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
        }

        [Test]
        public void GetTest_should_set_parent_filename_when_testfile_is_found()
        {
            // given + when
            var test = _testRepository.GetTest("parentFileName", It.IsAny<string>(), It.IsAny<int>());

            // then
            Assert.AreEqual("parentFileName", test.Filename);
            _fileHandler.Verify(x=>x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()),Times.Once);
            _fileHandler.Verify(x=>x.ReadAllText(It.IsAny<string>()),Times.Once);
        }

        [Test]
        public void SaveTest_should_throw_null_reference_exception_when_position_is_invalid()
        {
            // given + when + then
            Assert.Throws<ArgumentNullException>(() => _testRepository.SaveTest(null, It.IsAny<string>()));
        }

        [Test]
        public void SaveTest_should_return_true_when_testfile_is_saved()
        {
            // given + when
            bool success = _testRepository.SaveTest(new Test(), It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
            Assert.IsTrue(success);
        }

        [Test]
        public void CreateTest_should_throw_ArgumentNullException_when_test_is_null()
        {
            // given + when + then
            Assert.Throws<ArgumentNullException>(() => _testRepository.CreateTest(null, It.IsAny<string>()));
        }


        [Test]
        public void CreateTest_should_return_true_when_test_is_saved()
        {
            // given + when
            _testFileReader.Setup(x => x.Read(It.IsAny<TextReader>())).Returns(new TestFile());

            bool success = _testRepository.CreateTest(new Test(), It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
            Assert.IsTrue(success);
        }

        [Test]
        public void GetTestFile_should_return_testfile()
        {
            // given + when
            TestFile testFile = _testRepository.GetTestFile(It.IsAny<string>(), It.IsAny<string>());

            // then
            Assert.NotNull(testFile.Tests);
            Assert.AreEqual(1, testFile.Tests.Count());
        }

        [Test]
        public void DeleteTest_should_return_true_when_test_exists()
        {
            // given + when
            var success = _testRepository.DeleteTest(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
            _fileHandler.Verify(x=>x.WriteAllText(It.IsAny<string>(),It.IsAny<string>()));
            _testFileReader.Verify(x=>x.Read(It.IsAny<TextReader>()),Times.Once);
            Assert.IsTrue(success);
        }

        [Test]
        public void ListFilesForBranch_should_return_list_of_file_names()
        {
            // given + when
            IEnumerable<string> filenames = _testRepository.ListFilesForBranch(It.IsAny<string>());

            // then
            Assert.NotNull(filenames);
            Assert.AreEqual(1, filenames.Count());
            Assert.AreEqual("test", filenames.First());
        }

        [Test]
        public void CreateTestFile_should_throw_IO_exception_if_file_exists()
        {
            // given = when
            _fileHandler.Setup(x=>x.FileExists(It.IsAny<string>())).Returns(true);

            // then
            Assert.Throws<IOException>(()=>_testRepository.CreateTestFile(new TestFile {Filename="filePath.xml"}, It.IsAny<string>()));
        }

        [Test]
        public void CreateTestFile_should_return_true_if_file_does_not_exist()
        {
            // given + when
            _fileHandler.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);
            var testFile = _testRepository.CreateTestFile(new TestFile { Filename = "filePath.xml" }, It.IsAny<string>());

            // then
            Assert.IsTrue(testFile);
            _fileHandler.Verify(x => x.CreateFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.CreateFilename(It.IsAny<string>()), Times.Once);
            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
        }


        [Test]
        public void UpdateTestFile_should_return_true_if_file_exists()
        {
            // given + when
            bool success = _testRepository.UpdateTestFile(new TestFile { Filename = "filePath.xml" }, It.IsAny<string>());

            // then
            Assert.IsTrue(success);
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);

            _testFileWriter.Verify(x => x.Write(It.IsAny<TestFile>()), Times.Once);
            _testFileReader.Verify(x=>x.Read(It.IsAny<TextReader>()),Times.Once);
        }

        [Test]
        public void GetXml_should_return_correct_xml()
        {
            // given + when
            var xml = _testRepository.GetXml("filePath.xml", It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            Assert.AreEqual("<xml></xml>", xml);
        }
    }
}
