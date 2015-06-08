using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;

namespace Syringe.Tests.Unit.ModelBuilders
{
    [TestFixture]
    public class TestCaseViewModelBuilderTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuildTestCase_should_throw_argumentnullexception_when_testcase_is_null()
        {
            var testCaseViewModelBuilder = new TestCaseViewModelBuilder();
            testCaseViewModelBuilder.BuildTestCase(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BuildTestCases_should_throw_argumentnullexception_when_testcase_is_null()
        {
            var testCaseViewModelBuilder = new TestCaseViewModelBuilder();
            testCaseViewModelBuilder.BuildTestCases(null);
        }

        [Test]
        public void BuildTestCases_should_return_correct_model_values_from_casecollection()
        {
            // given
            var testCaseViewModelBuilder = new TestCaseViewModelBuilder();

            var caseCollection = new CaseCollection
            {
                TestCases = new List<Case>
                {
                    new Case {Id = 1, ShortDescription = "Short Description 1", Url = "http://www.google.com"},
                    new Case {Id = 2, ShortDescription = "Short Description 2", Url = "http://www.arsenal.com"},
                }
            };

            // when
            var testCaseViewModels = testCaseViewModelBuilder.BuildTestCases(caseCollection);

            // then
            Assert.NotNull(testCaseViewModels);
            Assert.That(testCaseViewModels.Count(), Is.EqualTo(2));

            var firstCase = testCaseViewModels.First();
            Assert.AreEqual(firstCase.Id, 1);
            Assert.AreEqual(firstCase.ShortDescription, "Short Description 1");
            Assert.AreEqual(firstCase.Url, "http://www.google.com");

            var lastCase = testCaseViewModels.Last();
            Assert.AreEqual(lastCase.Id, 2);
            Assert.AreEqual(lastCase.ShortDescription, "Short Description 2");
            Assert.AreEqual(lastCase.Url, "http://www.arsenal.com");
        }

        [Test]
        public void BuildTestCase_should_return_correct_model_values_from_case()
        {
            // given
            var testCaseViewModelBuilder = new TestCaseViewModelBuilder();

            var testCase = new Case
            {
                Id = 1,
                ShortDescription = "Short Description",
                Url = "http://www.google.com",
                ErrorMessage = "Error",
                LogRequest = true,
                LogResponse = true,
                LongDescription = "Long Description",
                Method = "Method",
                PostBody = "PostBody",
                PostType = PostType.GET.ToString(),
                VerifyResponseCode = HttpStatusCode.Accepted,
                Sleep = 10,

            };

            // when
            var testCaseViewModel = testCaseViewModelBuilder.BuildTestCase(testCase);

            // then
            Assert.NotNull(testCaseViewModel);
            Assert.AreEqual(testCaseViewModel.Id, testCase.Id);
            Assert.AreEqual(testCaseViewModel.ShortDescription, testCase.ShortDescription);
            Assert.AreEqual(testCaseViewModel.Url, testCase.Url);
            Assert.AreEqual(testCaseViewModel.ErrorMessage, testCase.ErrorMessage);
            Assert.AreEqual(testCaseViewModel.LogRequest, testCase.LogRequest);
            Assert.AreEqual(testCaseViewModel.LogResponse, testCase.LogResponse);
            Assert.AreEqual(testCaseViewModel.LongDescription, testCase.LongDescription);
            Assert.AreEqual(testCaseViewModel.Method, testCase.Method);
            Assert.AreEqual(testCaseViewModel.PostBody, testCase.PostBody);
            Assert.AreEqual(testCaseViewModel.PostType, PostType.GET);
            Assert.AreEqual(testCaseViewModel.VerifyResponseCode, testCase.VerifyResponseCode);
        }
    }
}
