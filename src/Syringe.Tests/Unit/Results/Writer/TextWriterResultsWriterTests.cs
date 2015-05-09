using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;

namespace Syringe.Tests.Unit.Results.Writer
{
	public class TextWriterResultsWriterTests
	{
		[Test]
		public void WriteHeader_should_append_message_and_seperator()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var stringWriter = new StringWriter(stringBuilder);
			var resultWriter = new TextWriterResultWriter(stringWriter);

			// Act
			resultWriter.WriteHeader("my string {0}", 1);

			// Assert
			string output = stringBuilder.ToString();

			Assert.That(output, Is.StringContaining(new String('-', 80)));
			Assert.That(output, Is.StringContaining("my string 1"));
		}

		[Test]
		public void Write_should_contain_test_and_result_information()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var stringWriter = new StringWriter(stringBuilder);
			var resultWriter = new TextWriterResultWriter(stringWriter);

			var result = new TestCaseResult()
			{
				ActualUrl = "http://www.actualurl.com",
				Message = "my message",
				ResponseTime = TimeSpan.FromSeconds(2),
				ResponseCodeSuccess = true,
				TestCase = new Case() {  Url = "http://www.originalurl.com", Id = 99, ShortDescription = "shortdescription" }
			};

			// Act
			resultWriter.Write(result);

			// Assert
			string output = stringBuilder.ToString();

			Assert.That(output, Is.StringContaining(new String('-', 80)));
			Assert.That(output, Is.StringContaining("Case 99 (shortdescription)"));
			Assert.That(output, Is.StringContaining(""));

			Assert.That(output, Is.StringContaining(" - Original url: " + result.TestCase.Url));
			Assert.That(output, Is.StringContaining(" - Actual url: " + result.ActualUrl));
			Assert.That(output, Is.StringContaining(" - Response code success: Passed"));
			Assert.That(output, Is.StringContaining(" - Time taken: " + result.ResponseTime));
			Assert.That(output, Is.StringContaining(" - Success: Passed"));
			Assert.That(output, Is.StringContaining(" - Message: " + result.Message));
		}

		[Test]
		public void Write_should_contain_verify_positive_results()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var stringWriter = new StringWriter(stringBuilder);
			var resultWriter = new TextWriterResultWriter(stringWriter);

			var result = new TestCaseResult()
			{
				TestCase = new Case() { Url = "", Id = 99, ShortDescription = "" }
			};
			result.VerifyPositiveResults.Add(new VerificationItem("desc1", "myregex1", VerifyType.Positive) { Success = false});
			result.VerifyPositiveResults.Add(new VerificationItem("desc2", "myregex2", VerifyType.Positive) { Success = true });

			// Act
			resultWriter.Write(result);

			// Assert
			string output = stringBuilder.ToString();

			Assert.That(output, Is.StringContaining("Verify positives"));
			Assert.That(output, Is.StringContaining("- Success: Failed"));
			Assert.That(output, Is.StringContaining("- desc1 - False"));
			Assert.That(output, Is.StringContaining("- desc2 - True"));
		}

		[Test]
		public void Write_should_contain_verify_negative_results()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var stringWriter = new StringWriter(stringBuilder);
			var resultWriter = new TextWriterResultWriter(stringWriter);

			var result = new TestCaseResult()
			{
				TestCase = new Case() { Url = "", Id = 99, ShortDescription = "" }
			};
			result.VerifyPositiveResults.Add(new VerificationItem("x1", "myregex1", VerifyType.Positive) { Success = true });
			result.VerifyPositiveResults.Add(new VerificationItem("x2", "myregex2", VerifyType.Positive) { Success = true });

			// Act
			resultWriter.Write(result);

			// Assert
			string output = stringBuilder.ToString();

			Assert.That(output, Is.StringContaining("Verify negatives"));
			Assert.That(output, Is.StringContaining("- Success: Passed"));
			Assert.That(output, Is.StringContaining("- x1 - True"));
			Assert.That(output, Is.StringContaining("- x2 - True"));
		}

		[Test]
		public void Write_should_contain_handle_empty_verify_results()
		{
			// Arrange
			var stringBuilder = new StringBuilder();
			var stringWriter = new StringWriter(stringBuilder);
			var resultWriter = new TextWriterResultWriter(stringWriter);

			var result = new TestCaseResult()
			{
				TestCase = new Case() { Url = "", Id = 99, ShortDescription = "" }
			};

			// Act
			resultWriter.Write(result);

			// Assert
			string output = stringBuilder.ToString();

			Assert.That(output, Is.StringContaining("Verify negatives"));
			Assert.That(output, Is.StringContaining("- (none found)"));
		}
	}
}
