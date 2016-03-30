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
			IEnumerable<AssertionViewModel> verifyPositives = GetVerificationItems(test.VerifyPositives);
			IEnumerable<AssertionViewModel> verifyNegatives = GetVerificationItems(test.VerifyNegatives);

			verifications.AddRange(verifyPositives);
			verifications.AddRange(verifyNegatives);

			var headerList = new List<Models.HeaderItem>(test.Headers.Select(x => new Models.HeaderItem { Key = x.Key, Value = x.Value }));
			var capturedVariables = new List<Models.CapturedVariableItem>(test.CapturedVariables.Select(x => new Models.CapturedVariableItem { Description = x.Name, Regex = x.Regex }));

			var model = new TestViewModel
			{
				Id = test.Id,
				ErrorMessage = test.ErrorMessage,
				Headers = headerList,
				LongDescription = test.LongDescription,
				CapturedVariables = capturedVariables,
				PostBody = test.PostBody,
				PostType = test.PostType == PostType.GET.ToString() ? PostType.GET : PostType.POST,
				VerifyResponseCode = test.VerifyResponseCode,
				ShortDescription = test.ShortDescription,
				Url = test.Url,
				Assertions = verifications,
				ParentFilename = test.ParentFilename,
			};

			return model;
		}

		private IEnumerable<AssertionViewModel> GetVerificationItems(IEnumerable<Assertion> items)
		{
			return
				items.Select(x =>
						new AssertionViewModel
						{
							Regex = x.Regex,
							Description = x.Description,
							AssertionType = x.AssertionType
						});
		}

		public IEnumerable<TestViewModel> BuildTestCases(IEnumerable<Test> tests)
		{
			if (tests == null)
			{
				throw new ArgumentNullException(nameof(tests));
			}

			return tests.Select(x => new TestViewModel()
			{
				Id = x.Id,
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
				Id = testModel.Id,
				ErrorMessage = testModel.ErrorMessage,
				Headers = testModel.Headers.Select(x => new HeaderItem(x.Key, x.Value)).ToList(),
				LongDescription = testModel.LongDescription,
				ParentFilename = testModel.ParentFilename,
				CapturedVariables = testModel.CapturedVariables.Select(x => new CapturedVariable(x.Description, x.Regex)).ToList(),
				PostBody = testModel.PostBody,
				VerifyPositives = testModel.Assertions.Where(x => x.AssertionType == AssertionType.Positive).Select(x => new Assertion(x.Description, x.Regex, x.AssertionType)).ToList(),
				VerifyNegatives = testModel.Assertions.Where(x => x.AssertionType == AssertionType.Negative).Select(x => new Assertion(x.Description, x.Regex, x.AssertionType)).ToList(),
				ShortDescription = testModel.ShortDescription,
				Url = testModel.Url,
				PostType = testModel.PostType.ToString(),
				VerifyResponseCode = testModel.VerifyResponseCode,
			};
		}
	}
}