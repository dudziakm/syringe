using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Syringe.Core.Xml;

namespace Syringe.Core.Runner
{
	internal class ParsedResponseMatcher
	{
		public static Dictionary<string, string> MatchParsedResponses(List<RegexItem> parsedResponses, string content)
		{
			var variables = new Dictionary<string, string>();

			foreach (RegexItem regexItem in parsedResponses)
			{
				Console.WriteLine("---------");
				Console.WriteLine("Testing {{parsedresponse{0}}}", regexItem.Description);
				Console.WriteLine(" - Regex: {0}", regexItem.Regex);

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
								Console.WriteLine(" - Found: {0}", capturedValue);
							}
						}
					}
					else
					{
						Console.WriteLine(" - No match");
					}
				}
				catch (ArgumentException e)
				{
					Console.WriteLine(" - Invalid regex: {0}", e.Message);
				}

				variables.Add("parsedresponse" + regexItem.Description, capturedValue);
			}

			return variables;
		}
	}
}