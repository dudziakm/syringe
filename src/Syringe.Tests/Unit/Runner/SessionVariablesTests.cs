﻿using System.Collections.Generic;
using NUnit.Framework;
using Syringe.Core.Runner;
using Syringe.Core.Tests;

namespace Syringe.Tests.Unit.Runner
{
	public class SessionVariablesTests
	{
		[Test]
		public void AddOrUpdateVariable_should_set_variable()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();

			// Act
			sessionVariables.AddOrUpdateVariable("nano", "leaf");

			// Assert
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
		}

		[Test]
		public void AddOrUpdateVariable_should_update_variable_when_already_set()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();

			// Act
			sessionVariables.AddOrUpdateVariable("nano", "leaf");
			sessionVariables.AddOrUpdateVariable("nano", "leaf2");

			// Assert
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf2"));
		}

		[Test]
		public void AddOrUpdateVariables_should_set_variable()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();

			// Act
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf", "env1"),
				new Variable("light", "bulb", "env2")
			});


			// Assert
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
			Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb"));
		}

		[Test]
		public void AddOrUpdateVariables_should_update_variable_when_already_set()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();

			// Act
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf", "env1"),
				new Variable("light", "bulb", "env2"),
			});
			sessionVariables.AddOrUpdateVariables(new List<Variable>()
			{
				new Variable("nano", "leaf2", "env1"),
				new Variable("light", "bulb2", "env2")
			});

			// Assert
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf2"));
			Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb2"));
		}

		[Test]
		public void ReplacePlainTextVariablesIn_should_replace_all_variables()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();
			sessionVariables.AddOrUpdateVariable("nano", "leaf");
			sessionVariables.AddOrUpdateVariable("two", "ten");

			string template = "{nano} {dummy} {two}";
			string expectedText = "leaf {dummy} ten";

			// Act
			string actualText = sessionVariables.ReplacePlainTextVariablesIn(template);

			// Assert
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		[Test]
		public void ReplaceVariablesIn_should_replace_all_variables_and_escape_regex_characters_in_values()
		{
			// Arrange
			var sessionVariables = new CapturedVariableProvider();
			sessionVariables.AddOrUpdateVariable("nano", "$var leaf");
			sessionVariables.AddOrUpdateVariable("two", "(.*?) [a-z] ^perlmagic");

			string template = "{nano} {dummy} {two}";
			string expectedText = @"\$var\ leaf {dummy} \(\.\*\?\)\ \[a-z]\ \^perlmagic";

			// Act
			string actualText = sessionVariables.ReplaceVariablesIn(template);

			// Assert
			Assert.That(actualText, Is.EqualTo(expectedText));
		}
	}
}