using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Syringe.Core.Logging;
using Syringe.Core.TestCases;

namespace Syringe.Core.Runner
{
	internal static class ParseResponseMatcher
	{
		/// <summary>
		/// Finds text in the content, returning them as variables, e.g. {parsedresponse1} = value
		/// </summary>
		public static List<Variable> MatchParseResponses(List<ParseResponseItem> parseResponses, string content, SimpleLogger simpleLogger)
		{
			var variables = new List<Variable>();

			foreach (ParseResponseItem regexItem in parseResponses)
			{
				simpleLogger.WriteLine("");
				simpleLogger.WriteLine("---------------------------");
				simpleLogger.WriteLine("Testing {{parsedresponse{0}}}", regexItem.Description);
				simpleLogger.WriteLine(" - Regex: {0}", regexItem.Regex);

				string capturedValue = "";
				try
				{
					var regex = new Regex(regexItem.Regex, RegexOptions.IgnoreCase);
					if (regex.IsMatch(content))
					{
						MatchCollection matches = regex.Matches(content);
						foreach (Match match in matches)
						{
							if (match.Groups.Count > 1)
							{
								capturedValue += match.Groups[1];
								Log.Information(" - Found: {0}", capturedValue);
							}
						}
					}
					else
					{
						simpleLogger.WriteLine(" - No match");
					}
				}
				catch (ArgumentException e)
				{
					simpleLogger.WriteLine(" - Invalid regex: {0}", e.Message);
				}

				variables.Add(new Variable("parsedresponse" + regexItem.Description, capturedValue));
			}

			return variables;
		}
	}
}