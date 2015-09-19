using System;
using System.Collections.Generic;
using System.Linq;
using Syringe.Core;
using Syringe.Core.TestCases;
using Syringe.Web.Models;
using VerificationItem = Syringe.Core.TestCases.VerificationItem;

namespace Syringe.Web.ModelBuilders
{
    public class TestCaseViewModelBuilder : ITestCaseViewModelBuilder
    {
        public TestCaseViewModel BuildTestCase(Case testCase)
        {
            if (testCase == null)
            {
                throw new ArgumentNullException("testCase");
            }

            var verifications = new List<Models.VerificationItemModel>();
            IEnumerable<Models.VerificationItemModel> verifyPositives = GetVerificationItems(testCase.VerifyPositives);
            IEnumerable<Models.VerificationItemModel> verifyNegatives = GetVerificationItems(testCase.VerifyNegatives);
            
            verifications.AddRange(verifyPositives);
            verifications.AddRange(verifyNegatives);

            var headerList = new List<Models.HeaderItem>(testCase.Headers.Select(x => new Models.HeaderItem { Key = x.Key, Value = x.Value }));
            var parsedResponses = new List<Models.ParseResponseItem>(testCase.ParseResponses.Select(x => new Models.ParseResponseItem { Description = x.Description, Regex = x.Regex }));

            var model = new TestCaseViewModel
            {
                Id = testCase.Id,
                ErrorMessage = testCase.ErrorMessage,
                Headers = headerList,
                LogRequest = testCase.LogRequest,
                LogResponse = testCase.LogResponse,
                LongDescription = testCase.LongDescription,
                Method = testCase.Method,
                ParseResponses = parsedResponses,
                PostBody = testCase.PostBody,
                PostType = testCase.PostType == PostType.GET.ToString() ? PostType.GET : PostType.POST,
                VerifyResponseCode = testCase.VerifyResponseCode,
                ShortDescription = testCase.ShortDescription,
                Sleep = testCase.Sleep,
                Url = testCase.Url,
                Verifications = verifications,
                ParentFilename = testCase.ParentFilename
            };

            return model;
        }

        private IEnumerable<Models.VerificationItemModel> GetVerificationItems(IEnumerable<VerificationItem> items)
        {
            return
                items.Select(x =>
                        new Models.VerificationItemModel
                        {
                            Regex = x.Regex,
                            Description = x.Description,
                            VerifyTypeValue = x.VerifyType.ToString(),
                            VerifyType = x.VerifyType
                        });
        }

        public IEnumerable<TestCaseViewModel> BuildTestCases(CaseCollection caseCollection)
        {
            if (caseCollection == null)
            {
                throw new ArgumentNullException("caseCollection");
            }

            return caseCollection.TestCases.Select(x => new TestCaseViewModel()
            {
                Id = x.Id,
                ShortDescription = x.ShortDescription,
                Url = x.Url
            });
        }
    }
}