using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Syringe.Core.Configuration;
using Syringe.Core.Runner;

namespace Syringe.Tests.Unit.Runner
{
	public class SessionVariablesTests
	{
		[Test]
		public void AddGlobalVariables_should_add_baseurl_as_variable()
		{
			// Arrange
			var config = new Config();
			config.BaseUrl = "mybaseurl";
			var sessionVariables = new SessionVariables();

			// Act
			sessionVariables.AddGlobalVariables(config);

			// Assert
			Assert.That(sessionVariables.GetVariableValue("baseurl"), Is.EqualTo("mybaseurl"));
		}

		[Test]
		public void AddGlobalVariables_should_add_variables_from_config()
		{
			// Arrange
			var config = new Config();
			config.Variables.Add(new Variable("eggs", "ham"));
			var sessionVariables = new SessionVariables();

			// Act
			sessionVariables.AddGlobalVariables(config);

			// Assert
			Assert.That(sessionVariables.GetVariableValue("eggs"), Is.EqualTo("ham"));
		}

		[Test]
		public void AddOrUpdateVariable_should_set_variable()
		{
			// Arrange
			var sessionVariables = new SessionVariables();

			// Act
			sessionVariables.AddOrUpdateVariable("nano", "leaf");

			// Assert
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
		}

		[Test]
		public void AddOrUpdateVariable_should_update_variable_when_already_set()
		{
			// Arrange
			var sessionVariables = new SessionVariables();

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
			var sessionVariables = new SessionVariables();

			// Act
			sessionVariables.AddOrUpdateVariables(new Dictionary<string, string>()
			{
				{"nano", "leaf"}, 
				{"light", "bulb"}
			});


			// Assert
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf"));
			Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb"));
		}

		[Test]
		public void AddOrUpdateVariables_should_update_variable_when_already_set()
		{
			// Arrange
			var sessionVariables = new SessionVariables();

			// Act
			sessionVariables.AddOrUpdateVariables(new Dictionary<string, string>()
			{
				{"nano", "leaf"}, 
				{"light", "bulb"},
			});
			sessionVariables.AddOrUpdateVariables(new Dictionary<string, string>()
			{
				{"nano", "leaf2"}, 
				{"light", "bulb2"}
			});


			// Assert
			Assert.That(sessionVariables.GetVariableValue("nano"), Is.EqualTo("leaf2"));
			Assert.That(sessionVariables.GetVariableValue("light"), Is.EqualTo("bulb2"));
		}
	}
}