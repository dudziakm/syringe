using NUnit.Framework;
using Syringe.Core.Results;
using Syringe.Core.TestCases;

namespace Syringe.Tests.Unit.Results
{
	public class TestCaseResultTests
	{
		[Test]
		[TestCase(true, true, true, true)]
		[TestCase(false, false, true, true)]
		[TestCase(false, false, true, true)]
		[TestCase(false, true, false, true)]
		[TestCase(false, true, true, false)]
		public void Success_should_return_result_based_on_success_codes(bool expectedResult, bool responseCodeSuccess, bool positiveSuccess, bool negativeSuccess)
		{
			// Arrange
			var testCaseResults = new TestCaseResult();
			testCaseResults.ResponseCodeSuccess = responseCodeSuccess;
			testCaseResults.VerifyPositiveResults.Add(new VerificationItem("desc", "regex", VerifyType.Positive) { Success = positiveSuccess });
			testCaseResults.VerifyNegativeResults.Add(new VerificationItem("desc", "regex", VerifyType.Negative) { Success = negativeSuccess });

			// Act
			bool actualResult = testCaseResults.Success;

			// Assert
			Assert.That(actualResult, Is.EqualTo(expectedResult));
		}

		[Test]
		public void VerifyPositivesSuccess_should_return_false_when_all_positive_results_are_false()
		{
			// Arrange
			var testCaseResults = new TestCaseResult();
			testCaseResults.VerifyPositiveResults.Add(new VerificationItem("desc", "regex", VerifyType.Positive) { Success = false });

			// Act
			bool actualResult = testCaseResults.VerifyPositivesSuccess;

			// Assert
			Assert.That(actualResult, Is.False);
		}

		[Test]
		public void VerifyNegativeSuccess_should_return_false_when_all_positive_results_are_false()
		{
			// Arrange
			var testCaseResults = new TestCaseResult();
			testCaseResults.VerifyNegativeResults.Add(new VerificationItem("desc", "regex", VerifyType.Negative) { Success = false });

			// Act
			bool actualResult = testCaseResults.VerifyNegativeSuccess;

			// Assert
			Assert.That(actualResult, Is.False);
		}

		[Test]
		public void VerifyPositivesSuccess_and_VerifyNegativeSuccess_should_return_true_when_positiveresults_is_null()
		{
			// Arrange
			var testCaseResults = new TestCaseResult();

			// Act + Assert
			Assert.That(testCaseResults.VerifyPositivesSuccess, Is.True);
			Assert.That(testCaseResults.VerifyNegativeSuccess, Is.True);
		}
	}
}
