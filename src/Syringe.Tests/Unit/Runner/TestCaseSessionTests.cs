using NUnit.Framework;
using Syringe.Core.Tests.Results;

namespace Syringe.Tests.Unit.Runner
{
	public class TestCaseSessionTests
	{
		[Test]
		public void TotalCasesPassed_should_count_passed_tests()
		{
			// Arrange
			var builder = new TestCaseResultsBuilder()
								.New().WithSuccess().Add()
								.New().WithSuccess().Add()
								.New().WithSuccess().Add()
								.New().WithFail().Add()
								.New().WithFail().Add();

			var testSession = new TestFileResult();
			testSession.TestResults = builder.GetCollection();

			// Act + Assert
			Assert.That(testSession.TotalTestsPassed, Is.EqualTo(3));
		}

		[Test]
		public void TotalCasesFailed_should_count_passed_tests()
		{
			// Arrange
			var builder = new TestCaseResultsBuilder()
								.New().WithSuccess().Add()
								.New().WithSuccess().Add()
								.New().WithFail().Add()
								.New().WithFail().Add()
								.New().WithFail().Add();

			var testSession = new TestFileResult();
			testSession.TestResults = builder.GetCollection();

			// Act + Assert
			Assert.That(testSession.TotalTestsFailed, Is.EqualTo(3));
		}

		[Test]
		public void TotalVerificationsPassed_should_count_passed_tests()
		{
			// Arrange
			var builder = new TestCaseResultsBuilder()
								.New().AddPositiveVerify().Add()
								.New().AddNegativeVerify().Add()
								.New().AddPositiveVerify(false).Add()
								.New().AddNegativeVerify(false).Add();

			var testSession = new TestFileResult();
			testSession.TestResults = builder.GetCollection();

			// Act + Assert
			Assert.That(testSession.TotalAssertionsPassed, Is.EqualTo(2));
		}

		[Test]
		public void TotalVerificationsFailed_should_count_failed_tests()
		{
			// Arrange
			var builder = new TestCaseResultsBuilder()
								.New().AddPositiveVerify().Add()
								.New().AddPositiveVerify(false).Add()
								.New().AddNegativeVerify(false).Add();

			var testSession = new TestFileResult();
			testSession.TestResults = builder.GetCollection();

			// Act + Assert
			Assert.That(testSession.TotalAssertionsFailed, Is.EqualTo(2));
		}
	}
}
