using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using Syringe.Core.IO;
using Syringe.Core.Repositories;
using Syringe.Core.Repositories.XML;
using Syringe.Core.TestCases;
using Syringe.Core.Xml.Reader;
using Syringe.Core.Xml.Writer;

namespace Syringe.Tests.Unit.Repositories
{
    [TestFixture]
    public class CaseRepositoryTests
    {
        private Mock<ITestCaseReader> _testCaseReader;
        private Mock<ITestCaseWriter> _testCaseWriter;
        private Mock<IFileHandler> _fileHandler;
        private CaseRepository _caseRepository;
        [SetUp]
        public void Setup()
        {
            _testCaseReader = new Mock<ITestCaseReader>();
            _testCaseWriter = new Mock<ITestCaseWriter>();
            _fileHandler = new Mock<IFileHandler>();

            _fileHandler.Setup(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>())).Returns("path");
            _fileHandler.Setup(x => x.CreateFileFullPath(It.IsAny<string>(), It.IsAny<string>())).Returns("filepath.xml");
            _fileHandler.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("<xml></xml>");
            _testCaseReader.Setup(x => x.Read(It.IsAny<TextReader>())).Returns(new CaseCollection { Filename="filepath.xml", TestCases = new List<Case> { new Case() } });
            _caseRepository = new CaseRepository(_testCaseReader.Object, _testCaseWriter.Object, _fileHandler.Object);
            _fileHandler.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _fileHandler.Setup(x => x.GetFileNames(It.IsAny<string>())).Returns(new List<string> {{"test"}});
            _testCaseWriter.Setup(x => x.Write(It.IsAny<CaseCollection>())).Returns("<xml></xml>");
        }

        [Test]
        public void GetTestCase_should_throw_null_reference_exception_when_position_is_invalid()
        {
            // given + when
            _testCaseReader.Setup(x => x.Read(It.IsAny<TextReader>())).Returns(new CaseCollection());

            // then
            Assert.Throws<NullReferenceException>(() => _caseRepository.GetTestCase(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
        }

        [Test]
        public void GetTestCase_should_set_parent_filename_when_testcase_is_found()
        {
            // given + when
            var testCase = _caseRepository.GetTestCase("parentFileName", It.IsAny<string>(), It.IsAny<int>());

            // then
            Assert.AreEqual("parentFileName", testCase.ParentFilename);
            _fileHandler.Verify(x=>x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()),Times.Once);
            _fileHandler.Verify(x=>x.ReadAllText(It.IsAny<string>()),Times.Once);
        }

        [Test]
        public void SaveTestCase_should_throw_null_reference_exception_when_position_is_invalid()
        {
            // given + when + then
            Assert.Throws<ArgumentNullException>(() => _caseRepository.SaveTestCase(null, It.IsAny<string>()));
        }

        [Test]
        public void SaveTestCase_should_return_true_when_testcase_is_saved()
        {
            // given + when
            var testCase = _caseRepository.SaveTestCase(new Case(), It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            _testCaseWriter.Verify(x => x.Write(It.IsAny<CaseCollection>()), Times.Once);
            Assert.IsTrue(testCase);
        }

        [Test]
        public void CreateTestCase_should_throw_ArgumentNullException_when_testcase_is_null()
        {
            // given + when + then
            Assert.Throws<ArgumentNullException>(() => _caseRepository.CreateTestCase(null, It.IsAny<string>()));
        }

        [Test]
        public void CreateTestCase_should_throw_exception_when_testcase_already_exists()
        {
            // given + when + then
            Assert.Throws<Exception>(() => _caseRepository.CreateTestCase(new Case(), It.IsAny<string>()),"case already exists");
        }

        [Test]
        public void CreateTestCase_should_return_true_when_testcase_is_saved()
        {
            // given + when
            _testCaseReader.Setup(x => x.Read(It.IsAny<TextReader>())).Returns(new CaseCollection());

            var testCase = _caseRepository.CreateTestCase(new Case(), It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            _testCaseWriter.Verify(x => x.Write(It.IsAny<CaseCollection>()), Times.Once);
            Assert.IsTrue(testCase);
        }

        [Test]
        public void GetTestCaseCollection_should_return_test_case_collection()
        {
            // given + when
            var testCase = _caseRepository.GetTestCaseCollection(It.IsAny<string>(), It.IsAny<string>());

            // then
            Assert.NotNull(testCase.TestCases);
            Assert.AreEqual(1, testCase.TestCases.Count());
        }

        [Test]
        public void DeleteTestCase_should_return_true_when_testCase_exists()
        {
            // given + when
            var testCase = _caseRepository.DeleteTestCase(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            _testCaseWriter.Verify(x => x.Write(It.IsAny<CaseCollection>()), Times.Once);
            _fileHandler.Verify(x=>x.WriteAllText(It.IsAny<string>(),It.IsAny<string>()));
            _testCaseReader.Verify(x=>x.Read(It.IsAny<TextReader>()),Times.Once);
            Assert.IsTrue(testCase);
        }

        [Test]
        public void DeleteTestCase_should_throw_null_reference_exception_when_test_case_is_missing()
        {
            // given + when + then
            _testCaseReader.Setup(x=>x.Read(It.IsAny<TextReader>())).Returns(new CaseCollection { TestCases = new List<Case>() });
            Assert.Throws<NullReferenceException>(()=>_caseRepository.DeleteTestCase(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void ListCasesForTeam_should_return_list_of_file_names()
        {
            // given + when
            var testCase = _caseRepository.ListCasesForTeam(It.IsAny<string>());

            // then
            Assert.NotNull(testCase);
            Assert.AreEqual(1, testCase.Count());
            Assert.AreEqual("test", testCase.First());
        }

        [Test]
        public void CreateTestFile_should_throw_IO_exception_if_file_exists()
        {
            // given = when
            _fileHandler.Setup(x=>x.FileExists(It.IsAny<string>())).Returns(true);

            // then
            Assert.Throws<IOException>(()=>_caseRepository.CreateTestFile(new CaseCollection {Filename="filePath.xml"}, It.IsAny<string>()));
        }

        [Test]
        public void CreateTestFile_should_return_true_if_file_does_not_exist()
        {
            // given + when
            _fileHandler.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);
            var testFile = _caseRepository.CreateTestFile(new CaseCollection { Filename = "filePath.xml" }, It.IsAny<string>());

            // then
            Assert.IsTrue(testFile);
            _fileHandler.Verify(x => x.CreateFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.CreateFilename(It.IsAny<string>()), Times.Once);
            _testCaseWriter.Verify(x => x.Write(It.IsAny<CaseCollection>()), Times.Once);
        }


        [Test]
        public void UpdateTestFile_should_return_true_if_file_exists()
        {
            // given + when
            var testFile = _caseRepository.UpdateTestFile(new CaseCollection { Filename = "filePath.xml" }, It.IsAny<string>());

            // then
            Assert.IsTrue(testFile);
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);

            _testCaseWriter.Verify(x => x.Write(It.IsAny<CaseCollection>()), Times.Once);
            _testCaseReader.Verify(x=>x.Read(It.IsAny<TextReader>()),Times.Once);
        }

        [Test]
        public void GetTestCaseCollection_should_return_correct_xml()
        {
            // given + when
            var xmlTestCaseCollection = _caseRepository.GetXmlTestCaseCollection("filePath.xml", It.IsAny<string>());

            // then
            _fileHandler.Verify(x => x.GetFileFullPath(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _fileHandler.Verify(x => x.ReadAllText(It.IsAny<string>()), Times.Once);
            Assert.AreEqual("<xml></xml>", xmlTestCaseCollection);
        }
    }
}
