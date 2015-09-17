using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Syringe.Core.Logging;
using Syringe.Core.TestCases;
using Syringe.Core.Xml;

namespace Syringe.Core.Runner
{
	internal static class ParseResponseMatcher
	{
		/// <summary>
		/// Finds text in the content, returning them as variables, e.g. {parsedresponse1} = value
		/// </summary>
		public static Dictionary<string, string> MatchParseResponses(List<ParseResponseItem> parseResponses, string content)
		{
			var variables = new Dictionary<string, string>();

			foreach (ParseResponseItem regexItem in parseResponses)
			{
				Log.Information("---------");
				Log.Information("Testing {{parsedresponse{0}}}", regexItem.Description);
				Log.Information(" - Regex: {0}", regexItem.Regex);

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
						Log.Information(" - No match");
					}
				}
				catch (ArgumentException e)
				{
					Log.Information(" - Invalid regex: {0}", e.Message);
				}

				variables.Add("parsedresponse" + regexItem.Description, capturedValue);
			}

			return variables;
		}
	}
}