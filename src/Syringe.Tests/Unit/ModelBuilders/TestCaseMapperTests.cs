using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Syringe.Core.TestCases;
using Syringe.Web.ModelBuilders;
using Syringe.Web.Models;
using HeaderItem = Syringe.Web.Models.HeaderItem;
using ParseResponseItem = Syringe.Web.Models.ParseResponseItem;
using Variables = Syringe.Web.Models.Variables;

namespace Syringe.Tests.Unit.ModelBuilders
{
    [TestFixture]
    public class TestCaseMapperTests
    {

        [Test]
        public void Build_should_set_correct_properties_when_model_is_populated()
        {
            var testCaseMapper = new TestCaseMapper();
            var build = testCaseMapper.BuildCoreModel(testCaseViewModel);

            Assert.AreEqual(testCaseViewModel.ErrorMessage, build.ErrorMessage);
            Assert.AreEqual(testCaseViewModel.Headers.Count, build.Headers.Count);
            Assert.AreEqual(testCaseViewModel.Id, build.Id);
            Assert.AreEqual(testCaseViewModel.LogRequest, build.LogRequest);
            Assert.AreEqual(testCaseViewModel.LogResponse, build.LogResponse);
            Assert.AreEqual(testCaseViewModel.LongDescription, build.LongDescription);
            Assert.AreEqual(testCaseViewModel.ParentFilename, build.ParentFilename);
            Assert.AreEqual(testCaseViewModel.ParseResponses.Count, build.ParseResponses.Count);
            Assert.AreEqual(testCaseViewModel.PostBody, build.PostBody);
            Assert.AreEqual(1, build.VerifyNegatives.Count);
            Assert.AreEqual(1, build.VerifyPositives.Count);
            Assert.AreEqual(testCaseViewModel.ShortDescription, build.ShortDescription);
            Assert.AreEqual(testCaseViewModel.Url, build.Url);
            Assert.AreEqual(testCaseViewModel.Sleep, build.Sleep);
            Assert.AreEqual(testCaseViewModel.PostType.ToString(), build.PostType);
            Assert.AreEqual(testCaseViewModel.VerifyResponseCode, build.VerifyResponseCode);
        }

        [Test]
        public void BuildTestCase_should_throw_argumentnullexception_when_testcase_is_null()
        {
            var testCaseMapper = new TestCaseMapper();

            Assert.Throws<ArgumentNullException>(() => testCaseMapper.BuildCoreModel(null));
        }

        [Test]
        public void BuildViewModel_should_throw_argumentnullexception_when_testcase_is_null()
        {
            var testCaseMapper = new TestCaseMapper();

            Assert.Throws<ArgumentNullException>(() => testCaseMapper.BuildViewModel(null));
        }

        [Test]
        public void BuildTestCases_should_throw_argumentnullexception_when_testcase_is_null()
        {
            var testCaseMapper = new TestCaseMapper();

            Assert.Throws<ArgumentNullException>(() => testCaseMapper.BuildTestCases(null));
        }

        [Test]
        public void BuildTestCases_should_return_correct_model_values_from_casecollection()
        {
            // given
            var testCaseMapper = new TestCaseMapper();
            var testCaseId = Guid.NewGuid();
            var testCaseId2 = Guid.NewGuid();
            var caseCollection = new CaseCollection
            {
                TestCases = new List<Case>
                {
                    new Case {Id = testCaseId, ShortDescription = "Short Description 1", Url = "http://www.google.com"},
                    new Case {Id = testCaseId2, ShortDescription = "Short Description 2", Url = "http://www.arsenal.com"},
                }
            };

            // when
            var testCaseViewModels = testCaseMapper.BuildTestCases(caseCollection.TestCases);

            // then
            Assert.NotNull(testCaseViewModels);
            Assert.AreEqual(2, testCaseViewModels.Count());

            var firstCase = testCaseViewModels.First();
            Assert.AreEqual(testCaseId, firstCase.Id);
            Assert.AreEqual("Short Description 1", firstCase.ShortDescription);
            Assert.AreEqual("http://www.google.com", firstCase.Url);

            var lastCase = testCaseViewModels.Last();
            Assert.AreEqual(testCaseId2, lastCase.Id);
            Assert.AreEqual("Short Description 2", lastCase.ShortDescription);
            Assert.AreEqual("http://www.arsenal.com", lastCase.Url);
        }

        [Test]
        public void BuildViewModel_should_return_correct_model_values_from_case()
        {
            // given
            var testCaseMapper = new TestCaseMapper();

            var testCase = new Case
            {
                Id = Guid.NewGuid(),
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
                Headers = new List<Core.TestCases.HeaderItem> { new Core.TestCases.HeaderItem() },
                ParseResponses = new List<Core.TestCases.ParseResponseItem> { new Core.TestCases.ParseResponseItem() },
                VerifyNegatives = new List<VerificationItem> { new VerificationItem() },
                VerifyPositives = new List<VerificationItem> { new VerificationItem() },
                ParentFilename = "test.xml",
                AvailableVariables = new List<Core.TestCases.Variables>() { new Core.TestCases.Variables() { Name = "Name", Value = "value", Type = "Type" } }
            };

            // when
            var testCaseViewModel = testCaseMapper.BuildViewModel(testCase);

            // then
            Assert.NotNull(testCaseViewModel);
            Assert.AreEqual(testCase.Id, testCaseViewModel.Id);
            Assert.AreEqual(testCase.ShortDescription, testCaseViewModel.ShortDescription);
            Assert.AreEqual(testCase.Url, testCaseViewModel.Url);
            Assert.AreEqual(testCase.ErrorMessage, testCaseViewModel.ErrorMessage);
            Assert.AreEqual(testCase.LogRequest, testCaseViewModel.LogRequest);
            Assert.AreEqual(testCase.LogResponse, testCaseViewModel.LogResponse);
            Assert.AreEqual(testCase.LongDescription, testCaseViewModel.LongDescription);
            Assert.AreEqual(testCase.PostBody, testCaseViewModel.PostBody);
            Assert.AreEqual(PostType.GET, testCaseViewModel.PostType);
            Assert.AreEqual(testCase.VerifyResponseCode, testCaseViewModel.VerifyResponseCode);
            Assert.AreEqual(testCase.ParentFilename, testCaseViewModel.ParentFilename);

            Assert.AreEqual(1, testCaseViewModel.ParseResponses.Count);
            Assert.AreEqual(2, testCaseViewModel.Verifications.Count);
            Assert.AreEqual(1, testCase.Headers.Count);
            Assert.AreEqual(1, testCase.AvailableVariables.Count);
        }


        private TestCaseViewModel testCaseViewModel
        {
            get
            {
                return new TestCaseViewModel
                {
                    ErrorMessage = "error",
                    Headers = new List<HeaderItem> { new HeaderItem { Key = "Key", Value = "Value" } },
                    Id = Guid.Empty,
                    LogRequest = true,
                    LogResponse = true,
                    LongDescription = "long description",
                    ParentFilename = "Test.xml",
                    ParseResponses = new List<ParseResponseItem>() { new ParseResponseItem { Description = "Description", Regex = "Regex" } },
                    PostBody = "Post Body",
                    Verifications = new List<VerificationItemModel>() { new VerificationItemModel { Description = "Description", Regex = "Regex", VerifyType = VerifyType.Negative },
                               new VerificationItemModel { Description = "Description", Regex = "Regex", VerifyType = VerifyType.Positive } },
                    ShortDescription = "short d3escription",
                    Url = "url",
                    Sleep = 10,
                    PostType = PostType.POST,
                    VerifyResponseCode = HttpStatusCode.Accepted,
                    AvailableVariables = new List<Variables>() { new Variables() {Name = "Name",Value="value", Type = "Type"} }
                };
            }
        }
    }
}
