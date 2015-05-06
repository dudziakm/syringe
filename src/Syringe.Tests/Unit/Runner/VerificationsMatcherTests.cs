using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Syringe.Core;
using Syringe.Core.Logging;
using Syringe.Core.Runner;

namespace Syringe.Tests.Unit.Runner
{
	public class VerificationsMatcherTests
	{
		[Test]
		public void badregex_should_set_success_to_false()
		{
			// Arrange
			Log.UseConsole();
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
		public void should_match_positives()
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
	}
}