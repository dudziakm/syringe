using System;
using System.Collections.Generic;
using System.Linq;
using Syringe.Core.TestCases;
using Syringe.Web.Models;
using HeaderItem = Syringe.Core.TestCases.HeaderItem;
using ParseResponseItem = Syringe.Core.TestCases.ParseResponseItem;
using Variables = Syringe.Web.Models.Variables;
using VerificationItem = Syringe.Core.TestCases.VerificationItem;

namespace Syringe.Web.ModelBuilders
{
    public class TestCaseMapper : ITestCaseMapper
    {
        public TestCaseViewModel BuildViewModel(Case testCase)
        {
            if (testCase == null)
            {
                throw new ArgumentNullException("testCase");
            }

            var verifications = new List<VerificationItemModel>();
            IEnumerable<VerificationItemModel> verifyPositives = GetVerificationItems(testCase.VerifyPositives);
            IEnumerable<VerificationItemModel> verifyNegatives = GetVerificationItems(testCase.VerifyNegatives);
            
            verifications.AddRange(verifyPositives);
            verifications.AddRange(verifyNegatives);

            var headerList = new List<Models.HeaderItem>(testCase.Headers.Select(x => new Models.HeaderItem { Key = x.Key, Value = x.Value }));
            var parsedResponses = new List<Models.ParseResponseItem>(testCase.ParseResponses.Select(x => new Models.ParseResponseItem { Description = x.Description, Regex = x.Regex }));

            var model = new TestCaseViewModel
            {
                Id = testCase.Id,
                ErrorMessage = testCase.ErrorMessage,
                Headers = headerList,
                LongDescription = testCase.LongDescription,
                ParseResponses = parsedResponses,
                PostBody = testCase.PostBody,
                PostType = testCase.PostType == PostType.GET.ToString() ? PostType.GET : PostType.POST,
                VerifyResponseCode = testCase.VerifyResponseCode,
                ShortDescription = testCase.ShortDescription,
                Sleep = testCase.Sleep,
                Url = testCase.Url,
                Verifications = verifications,
                ParentFilename = testCase.ParentFilename,
                AvailableVariables = testCase.AvailableVariables.Select(x=> new Variables { Name=x.Name,Type=x.Type,Value=x.Value}).ToList()
            };

            return model;
        }

        private IEnumerable<VerificationItemModel> GetVerificationItems(IEnumerable<VerificationItem> items)
        {
            return
                items.Select(x =>
                        new VerificationItemModel
                        {
                            Regex = x.Regex,
                            Description = x.Description,
                            VerifyType = x.VerifyType
                        });
        }

        public IEnumerable<TestCaseViewModel>  BuildTestCases(IEnumerable<Case> cases)
        {
            if (cases == null)
            {
                throw new ArgumentNullException("cases");
            }

            return cases.Select(x => new TestCaseViewModel()
            {
                Id = x.Id,
                ShortDescription = x.ShortDescription,
                Url = x.Url
            });
        }

        public Case BuildCoreModel(TestCaseViewModel testCase)
        {
            if (testCase == null)
            {
                throw new ArgumentNullException("testCase");
            }

            return new Case
            {
                Id = testCase.Id,
                ErrorMessage = testCase.ErrorMessage,
                Headers = testCase.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList(),
                LongDescription = testCase.LongDescription,
                ParentFilename = testCase.ParentFilename,
                ParseResponses = testCase.ParseResponses.Select(x => new ParseResponseItem(x.Description, x.Regex)).ToList(),
                PostBody = testCase.PostBody,
                VerifyPositives = testCase.Verifications.Where(x => x.VerifyType == VerifyType.Positive).Select(x => new VerificationItem(x.Description, x.Regex, x.VerifyType)).ToList(),
                VerifyNegatives = testCase.Verifications.Where(x => x.VerifyType == VerifyType.Negative).Select(x => new VerificationItem(x.Description, x.Regex, x.VerifyType)).ToList(),
                ShortDescription = testCase.ShortDescription,
                Url = testCase.Url,
                Sleep = testCase.Sleep,
                PostType = testCase.PostType.ToString(),
                VerifyResponseCode = testCase.VerifyResponseCode,
            };
        }
    }
}