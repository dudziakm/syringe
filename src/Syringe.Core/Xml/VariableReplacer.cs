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

			//Posted+{DAY}%2F{MONTH}%2F{YEAR}+{HH}%3A{MM}%3A{SS}
			//{WEEKOFMONTH}                                       Week number - used to access correct folder in error logs
			//{DATETIME}                                          Date and time as a single number, unformatted
			//{FORMATDATETIME}                                    Date and time, formatted with slashes and colons
			//{OUTPUT}                                            Temporary Folder - relative path from Current Working Directory e.g. .\temp\A\
			//{CONCURRENCY}                                       Temporary Folder - name only, not full part e.g. A
			//{CWD}                                               Current Working Directory - absolute path
			//{OUTSUM}                                            Hash of temporary folder name for concurrency
			//{COUNTER}                                           How many times the testcases have looped
			//{STARTTIME}                                         Number of elapsed seconds from a certain date till webinject started
			//{TIMESTAMP}                                         Number representing current time
			//{AMPERSAND}
			//{LESSTHAN}
			//{OUTSUM}-{DAY}{MONTH}-{HH}{MM}{SS}                  Unique Key - use dashes, not underscores
			//{LENGTH}                                            Length of the previous test step response
			//{TESTNUM}                                           Test step id of this step, e.g. 3020
			//{TESTSTEPTIME:3020}                                 Latency of prevously executed step 3020
			//{ELAPSED_SECONDS}                                   Seconds elapsed since test started
			//{ELAPSED_MINUTES}                                   Minutes elapsed since test started
			//{RETRY}                                             Number of times this step has been retried
		}

		private void ReplaceValue(string variableName, object variableValue)
		{
			//_stringBuilder.Replace("{" + variableName + "}", variableValue.ToString());
		}
	}
}
