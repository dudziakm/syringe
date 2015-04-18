using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syringe.Core.Configuration;

namespace Syringe.Core.Xml
{
	internal class VariableReplacer
	{
		private readonly Config _config;
		private readonly TestCaseCollection _testCollection;
		private StringBuilder _stringBuilder;

		public VariableReplacer(Config config, TestCaseCollection testCollection)
		{
			_config = config;
			_testCollection = testCollection;
		}

		private void Replace()
		{
			foreach (TestCase testCase in _testCollection.TestCases)
			{
				_stringBuilder = new StringBuilder("");

				ReplaceCustomVariables();
			}
		}

		private void ReplaceCustomVariables()
		{
			foreach (Variable variable in _config.Variables)
			{
				ReplaceValue(variable.Name, _testCollection.Variables[variable.Value]);
			}

			foreach (string key in _testCollection.Variables.Keys)
			{
				ReplaceValue(key, _testCollection.Variables[key]);
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
