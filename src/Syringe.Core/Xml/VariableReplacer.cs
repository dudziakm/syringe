using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syringe.Core.Xml
{
	internal class VariableReplacer
	{
		private readonly Config _config;
		private readonly TestCaseContainer _container;
		private StringBuilder _stringBuilder;

		public VariableReplacer(Config config, TestCaseContainer container)
		{
			_config = config;
			_container = container;
		}

		private void Replace()
		{
			foreach (TestCase testCase in _container.TestCases)
			{
				_stringBuilder = new StringBuilder("");

				ReplaceCustomVariables();
			}
		}

		private void ReplaceCustomVariables()
		{
			foreach (string key in _config.Variables.Keys)
			{
				ReplaceValue(key, _container.Variables[key]);
			}

			foreach (string key in _container.Variables.Keys)
			{
				ReplaceValue(key, _container.Variables[key]);
			}
		}

		private void ReplaceValue(string variableName, object variableValue)
		{
			_stringBuilder.Replace("{" + variableName + "}", variableValue.ToString());
		}
	}

	public class ConstantsReplacer
	{

		private void ReplaceBuiltInVariables()
		{
			//{DAY}.{MONTH}.{YEAR} {HH}:{MM}:{SS}
			ReplaceValue("DAY", DateTime.Now.Day);
			ReplaceValue("MONTH", DateTime.Now.Month);
			ReplaceValue("YEAR", DateTime.Now.Year);
			ReplaceValue("HH", DateTime.Now.Hour);
			ReplaceValue("SS", DateTime.Now.Second);
		}

		private void ReplaceValue(string variableName, object variableValue)
		{
			//_stringBuilder.Replace("{" + variableName + "}", variableValue.ToString());
		}
	}
}
