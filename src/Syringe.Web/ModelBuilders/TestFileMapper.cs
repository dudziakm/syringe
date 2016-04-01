using System;
using System.Collections.Generic;
using System.Linq;
using Syringe.Core.Tests;
using Syringe.Web.Models;
using HeaderItem = Syringe.Core.Tests.HeaderItem;

namespace Syringe.Web.ModelBuilders
{
    public class TestFileMapper : ITestFileMapper
    {
        public TestViewModel BuildViewModel(Test test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            var verifications = new List<AssertionViewModel>();
            var verifyPositives = test.Assertions.Select(x => new AssertionViewModel { Regex = x.Regex, Description = x.Description, AssertionType = x.AssertionType });

            verifications.AddRange(verifyPositives);

            var headerList = new List<Models.HeaderItem>(test.Headers.Select(x => new Models.HeaderItem { Key = x.Key, Value = x.Value }));
            var capturedVariables = new List<CapturedVariableItem>(test.CapturedVariables.Select(x => new CapturedVariableItem { Description = x.Name, Regex = x.Regex }));

            var model = new TestViewModel
            {
                Position = test.Position,
                ErrorMessage = test.ErrorMessage,
                Headers = headerList,
                LongDescription = test.LongDescription,
                CapturedVariables = capturedVariables,
                PostBody = test.PostBody,
                PostType = (PostType)Enum.Parse(typeof(PostType), test.PostType),
                VerifyResponseCode = test.VerifyResponseCode,
                ShortDescription = test.ShortDescription,
                Url = test.Url,
                Assertions = verifications,
                Filename = test.Filename,
                AvailableVariables = test.AvailableVariables.Select(x => new VariableViewModel { Name = x.Name, Value = x.Value }).ToList()
            };

            return model;
        }

        public IEnumerable<TestViewModel> BuildTests(IEnumerable<Test> tests)
        {
            if (tests == null)
            {
                throw new ArgumentNullException(nameof(tests));
            }

            return tests.Select(x => new TestViewModel()
            {
                Position = x.Position,
                ShortDescription = x.ShortDescription,
                Url = x.Url
            });
        }

        public Test BuildCoreModel(TestViewModel testModel)
        {
            if (testModel == null)
            {
                throw new ArgumentNullException(nameof(testModel));
            }

            return new Test
            {
                Position = testModel.Position,
                ErrorMessage = testModel.ErrorMessage,
                Headers = testModel.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList(),
                LongDescription = testModel.LongDescription,
                Filename = testModel.Filename,
                CapturedVariables = testModel.CapturedVariables.Select(x => new CapturedVariable(x.Description, x.Regex)).ToList(),
                PostBody = testModel.PostBody,
                Assertions = testModel.Assertions.Select(x => new Assertion(x.Description, x.Regex, x.AssertionType)).ToList(),
                ShortDescription = testModel.ShortDescription,
                Url = testModel.Url,
                PostType = testModel.PostType.ToString(),
                VerifyResponseCode = testModel.VerifyResponseCode,
            };
        }
    }
}