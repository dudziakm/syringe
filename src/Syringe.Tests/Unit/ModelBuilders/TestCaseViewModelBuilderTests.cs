using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;
using ParseResponseItem = Syringe.Core.ParseResponseItem;
using VerificationItem = Syringe.Core.VerificationItem;

namespace Syringe.Tests.Unit.ModelBuilders
{
    [TestFixture]
    public class TestCaseViewModelBuilderTests
    {
        [Test]
        public void BuildTestCase_should_throw_argumentnullexception_when_testcase_is_null()
        {
            var testCaseViewModelBuilder = new TestCaseViewModelBuilder();

			Assert.Throws<ArgumentNullException>(() => testCaseViewModelBuilder.BuildTestCase(null));
        }

        [Test]
        public void BuildTestCases_should_throw_argumentnullexception_when_testcase_is_null()
        {
            var testCaseViewModelBuilder = new TestCaseViewModelBuilder();

			Assert.Throws<ArgumentNullException>(() => testCaseViewModelBuilder.BuildTestCase(null));
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
			Assert.AreEqual(2, testCaseViewModels.Count());

			var firstCase = testCaseViewModels.First();
			Assert.AreEqual(1, firstCase.Id);
			Assert.AreEqual("Short Description 1", firstCase.ShortDescription);
			Assert.AreEqual("http://www.google.com", firstCase.Url);

			var lastCase = testCaseViewModels.Last();
			Assert.AreEqual(2, lastCase.Id);
			Assert.AreEqual("Short Description 2", lastCase.ShortDescription);
			Assert.AreEqual("http://www.arsenal.com", lastCase.Url);
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
                Headers = new List<KeyValuePair<string, string>>{new KeyValuePair<string, string>()},
                ParseResponses = new List<ParseResponseItem>{new ParseResponseItem()},
                VerifyNegatives = new List<VerificationItem>{new VerificationItem()},
                VerifyPositives = new List<VerificationItem>{new VerificationItem()},
                ParentFilename = "test.xml"
            };

            // when
            var testCaseViewModel = testCaseViewModelBuilder.BuildTestCase(testCase);

            // then
            Assert.NotNull(testCaseViewModel);
			Assert.AreEqual(testCase.Id, testCaseViewModel.Id);
			Assert.AreEqual(testCase.ShortDescription, testCaseViewModel.ShortDescription);
			Assert.AreEqual(testCase.Url, testCaseViewModel.Url);
			Assert.AreEqual(testCase.ErrorMessage, testCaseViewModel.ErrorMessage);
			Assert.AreEqual(testCase.LogRequest, testCaseViewModel.LogRequest);
			Assert.AreEqual(testCase.LogResponse, testCaseViewModel.LogResponse);
			Assert.AreEqual(testCase.LongDescription, testCaseViewModel.LongDescription);
			Assert.AreEqual(testCase.Method, testCaseViewModel.Method);
			Assert.AreEqual(testCase.PostBody, testCaseViewModel.PostBody);
			Assert.AreEqual(PostType.GET, testCaseViewModel.PostType);
			Assert.AreEqual(testCase.VerifyResponseCode, testCaseViewModel.VerifyResponseCode);
			Assert.AreEqual(testCase.ParentFilename, testCaseViewModel.ParentFilename);

			Assert.AreEqual(1, testCaseViewModel.ParseResponses.Count);
			Assert.AreEqual(2, testCaseViewModel.Verifications.Count);
        }
    }
}
