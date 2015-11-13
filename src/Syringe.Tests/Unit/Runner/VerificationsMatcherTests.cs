using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Logging;
using Syringe.Core.Runner;
using Syringe.Core.TestCases;

namespace Syringe.Tests.Unit.Runner
{
	public class VerificationsMatcherTests
	{
		[SetUp]
		public void Setup()
		{
			TestHelpers.EnableLogging();
		}

		[Test]
		public void invalid_regex_should_set_success_to_false()
		{
			// Arrange
			var sessionVariables = new SessionVariables();
			var matcher = new VerificationsMatcher(sessionVariables);

			var verifications = new List<VerificationItem>();
			verifications.Add(new VerificationItem("dodgy regex", "((*)", VerifyType.Positive));

			string content = "<p>Some content here</p>";

			// Act
			List<VerificationItem> results = matcher.MatchPositive(verifications, content);

			// Assert
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Success, Is.False);
		}

		[Test]
		public void MatchNegative_should_return_veriftype_positives_in_list()
		{
			// Arrange
			var sessionVariables = new SessionVariables();
			var matcher = new VerificationsMatcher(sessionVariables);

			var verifications = new List<VerificationItem>();
			verifications.Add(new VerificationItem("p1", "a regex", VerifyType.Positive));
			verifications.Add(new VerificationItem("p2", "another regex", VerifyType.Positive));
			verifications.Add(new VerificationItem("n1", "one more regex", VerifyType.Negative));


			string content = "<p>whatever</p>";

			// Act
			List<VerificationItem> results = matcher.MatchNegative(verifications, content);

			// Assert
			Assert.That(results.Count, Is.EqualTo(3));
		}

		[Test]
		public void MatchPositive_should_match_text_in_content()
		{
			// Arrange
			var sessionVariables = new SessionVariables();
			var matcher = new VerificationsMatcher(sessionVariables);

			var verifications = new List<VerificationItem>();
			verifications.Add(new VerificationItem("desc1","content here", VerifyType.Positive));
			verifications.Add(new VerificationItem("desc2", "bad regex", VerifyType.Positive));

			string content = "<p>Some content here</p>";

			// Act
			List<VerificationItem> results = matcher.MatchPositive(verifications, content);

			// Assert
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results[0].Success, Is.True);
			Assert.That(results[0].Description, Is.EqualTo("desc1"));
			Assert.That(results[0].Regex, Is.EqualTo("content here"));

			Assert.That(results[1].Success, Is.False);
			Assert.That(results[1].Description, Is.EqualTo("desc2"));
			Assert.That(results[1].Regex, Is.EqualTo("bad regex"));
		}

		[Test]
		public void MatchNegative_should_not_match_text_that_is_not_in_content()
		{
			// Arrange
			var sessionVariables = new SessionVariables();
			var matcher = new VerificationsMatcher(sessionVariables);

			var verifications = new List<VerificationItem>();
			verifications.Add(new VerificationItem("desc1", "this isnt in the text", VerifyType.Negative));
			verifications.Add(new VerificationItem("desc2", "content here", VerifyType.Negative));

			string content = "<p>Some content here</p>";

			// Act
			List<VerificationItem> results = matcher.MatchNegative(verifications, content);

			// Assert
			Assert.That(results.Count, Is.EqualTo(2));
			Assert.That(results[0].Success, Is.True);
			Assert.That(results[0].Description, Is.EqualTo("desc1"));
			Assert.That(results[0].Regex, Is.EqualTo("this isnt in the text"));

			Assert.That(results[1].Success, Is.False);
			Assert.That(results[1].Description, Is.EqualTo("desc2"));
			Assert.That(results[1].Regex, Is.EqualTo("content here"));
		}

		[Test]
		public void should_replace_variables_in_regex()
		{
			// Arrange
			var sessionVariables = new SessionVariables();
			sessionVariables.AddOrUpdateVariable("password", "tedx123");

			var matcher = new VerificationsMatcher(sessionVariables);

			var verifications = new List<VerificationItem>();
			verifications.Add(new VerificationItem("desc1", "({password})", VerifyType.Positive));

			string content = "<p>The password is tedx123</p>";

			// Act
			List<VerificationItem> results = matcher.MatchPositive(verifications, content);

			// Assert
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results[0].Success, Is.True);
			Assert.That(results[0].Description, Is.EqualTo("desc1"));
			Assert.That(results[0].Regex, Is.EqualTo("({password})"));
			Assert.That(results[0].TransformedRegex, Is.EqualTo("(tedx123)"));
		}
	}
}