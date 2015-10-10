using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using Syringe.Core.FileOperations;
using Syringe.Core.Repositories;
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

            _fileHandler.Setup(x => x.GetFullPath(It.IsAny<string>(), It.IsAny<string>())).Returns("path");
            _fileHandler.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("<xml></xml>");
            _testCaseReader.Setup(x => x.Read(It.IsAny<TextReader>())).Returns(new CaseCollection { TestCases = new List<Case> { new Case() } });
            _caseRepository = new CaseRepository(_testCaseReader.Object, _testCaseWriter.Object, _fileHandler.Object);
            _fileHandler.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        }

        [Test]
        public void GetTestCase_should_throw_null_reference_exception_when_caseId_is_invalid()
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
        }

        [Test]
        public void SaveTestCase_should_throw_null_reference_exception_when_caseId_is_invalid()
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
            Assert.IsTrue(testCase);
        }
    }
}
