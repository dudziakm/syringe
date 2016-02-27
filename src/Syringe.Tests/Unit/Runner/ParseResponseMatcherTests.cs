using System.Collections.Generic;
using NUnit.Framework;
using Syringe.Core.Runner;
using Syringe.Core.TestCases;

namespace Syringe.Tests.Unit.Runner
{
	public class ParseResponseMatcherTests
	{
		[SetUp]
		public void Setup()
		{
			TestHelpers.EnableLogging();
        }

		[Test]
		public void should_match_regex_groups_and_set_variable_names_and_values_to_matched_items()
		{
			// Arrange
			var parseResponses = new List<ParseResponseItem>()
			{
				new ParseResponseItem("1", @"(\d+)"),
				new ParseResponseItem("foo", "(<html.+?>)")
			};
			string content = "<html class='bootstrap'><p>Tap tap tap 123</p></html>";

			// Act
			Dictionary<string, string> variables = ParseResponseMatcher.MatchParseResponses(parseResponses, content);

			// Assert
			Assert.That(variables.Count, Is.EqualTo(2));
			Assert.That(variables["parsedresponse1"], Is.EqualTo("123"));
			Assert.That(variables["parsedresponsefoo"], Is.EqualTo("<html class='bootstrap'>"));
		}

		[Test]
		public void should_set_value_to_empty_string_when_regex_is_not_matched()
		{
			// Arrange
			var parseResponses = new List<ParseResponseItem>()
			{
				new ParseResponseItem("1", @"foo"),
				new ParseResponseItem("2", @"bar"),

			};
			string content = "<html>123 abc</html>";

			// Act
			Dictionary<string, string> variables = ParseResponseMatcher.MatchParseResponses(parseResponses, content);

			// Assert
			Assert.That(variables.Count, Is.EqualTo(2));
			Assert.That(variables["parsedresponse1"], Is.EqualTo(""));
			Assert.That(variables["parsedresponse2"], Is.EqualTo(""));
		}

		[Test]
		public void should_set_value_to_empty_string_when_regex_is_invalid()
		{
			// Arrange
			var parseResponses = new List<ParseResponseItem>()
			{
				new ParseResponseItem("1", @"(\d+)"),
				new ParseResponseItem("2", @"(() this is a bad regex?("),

			};
			string content = "<html>123 abc</html>";

			// Act
			Dictionary<string, string> variables = ParseResponseMatcher.MatchParseResponses(parseResponses, content);

			// Assert
			Assert.That(variables.Count, Is.EqualTo(2));
			Assert.That(variables["parsedresponse1"], Is.EqualTo("123"));
			Assert.That(variables["parsedresponse2"], Is.EqualTo(""));
		}

		[Test]
		public void should_concatenate_multiple_matches_into_variable_value()
		{
			// Arrange
			var parseResponses = new List<ParseResponseItem>()
			{
				new ParseResponseItem("1", @"(\d+)"),
			};
			string content = "<html>The number 3 and the number 4 combined make 7</html>";

			// Act
			Dictionary<string, string> variables = ParseResponseMatcher.MatchParseResponses(parseResponses, content);

			// Assert
			Assert.That(variables["parsedresponse1"], Is.EqualTo("347"));
		}
	}
}