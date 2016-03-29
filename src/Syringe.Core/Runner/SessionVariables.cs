using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Syringe.Core.Logging;
using Syringe.Core.TestCases;
using Syringe.Core.TestCases.Configuration;

namespace Syringe.Core.Runner
{
	internal class SessionVariables
	{
		private readonly List<Variable> _currentVariables;

		public SessionVariables()
		{
			_currentVariables = new List<Variable>();
		}

		public void AddGlobalVariables(Config config)
		{
			if (!string.IsNullOrEmpty(config.BaseUrl))
				_currentVariables.Add(new Variable("baseurl", config.BaseUrl, ""));

			foreach (Variable variable in config.Variables)
			{
				_currentVariables.Add(new Variable(variable.Name, variable.Value, ""));
			}
		}

		public void AddOrUpdateVariables(List<Variable> variables)
		{
			foreach (Variable variable in variables)
			{
				AddOrUpdateVariable(variable.Name, variable.Value);
			}
		}

		public void AddOrUpdateVariable(string name, string value)
		{
			var variable = _currentVariables.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if (variable != null)
			{
				variable.Value = value;
			}
			else
			{
				_currentVariables.Add(new Variable(name, value, ""));
			}
		}

		public string GetVariableValue(string name)
		{
			var variable = _currentVariables.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

			if (variable != null)
				return variable.Value;

			return "";
		}

		public string ReplacePlainTextVariablesIn(string text)
		{
			foreach (Variable variable in _currentVariables)
			{
				text = text.Replace("{" + variable.Name + "}", variable.Value);
			}

			return text;
		}

		public string ReplaceVariablesIn(string text)
		{
			foreach (Variable variable in _currentVariables)
			{
				text = text.Replace("{" + variable.Name + "}", Regex.Escape(variable.Value));
			}

			return text;
		}

		public void Dump()
		{
			Log.Information("In my bag of magic variables I have:");

			foreach (Variable variable in _currentVariables)
			{
				Log.Information(" - {{{0}}} : {1}", variable.Name, variable.Value);
			}
		}
	}
}