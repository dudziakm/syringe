using System;
using System.Linq;
using Syringe.Core;
using Syringe.Web.Models;

namespace Syringe.Web.ModelBuilders
{
	public class TestCaseCoreModelBuilder : ITestCaseCoreModelBuilder
	{
		public Case Build(TestCaseViewModel testCase)
		{
			if (testCase == null)
			{
				throw new ArgumentNullException("testCase");
			}

			return new Case
			       {
				       Id = testCase.Id,
				       ErrorMessage = testCase.ErrorMessage,
				       Headers = testCase.Headers.Select(x => new Core.HeaderItem(x.Key, x.Value)).ToList(),
				       LogRequest = testCase.LogRequest,
				       LogResponse = testCase.LogResponse,
				       LongDescription = testCase.LongDescription,
				       Method = testCase.Method,
				       ParentFilename = testCase.ParentFilename,
				       ParseResponses = testCase.ParseResponses.Select(x => new Core.ParseResponseItem(x.Description, x.Regex)).ToList(),
				       PostBody = testCase.PostBody,
				       VerifyPositives = testCase.Verifications.Where(x => x.VerifyType == VerifyType.Positive).Select(x => new Core.VerificationItem(x.Description, x.Regex, x.VerifyType)).ToList(),
				       VerifyNegatives = testCase.Verifications.Where(x => x.VerifyType == VerifyType.Negative).Select(x => new Core.VerificationItem(x.Description, x.Regex, x.VerifyType)).ToList(),
				       ShortDescription = testCase.ShortDescription,
				       Url = testCase.Url,
				       Sleep = testCase.Sleep,
				       PostType = testCase.PostType.ToString(),
				       VerifyResponseCode = testCase.VerifyResponseCode,
			       };
		}
	}
}