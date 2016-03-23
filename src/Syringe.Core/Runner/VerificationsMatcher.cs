using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Syringe.Core.Logging;
using Syringe.Core.TestCases;

namespace Syringe.Core.Runner
{
	internal class VerificationsMatcher
	{
		private readonly SessionVariables _variables;

		internal enum VerificationBehaviour
		{
			Negative,
			Positive
		}

		public VerificationsMatcher(SessionVariables variables)
		{
			_variables = variables;
		}

		public List<VerificationItem> MatchPositive(List<VerificationItem> verifications, string content)
		{
			return MatchVerifications(verifications, content, VerificationBehaviour.Positive);
		}

		public List<VerificationItem> MatchNegative(List<VerificationItem> verifications, string content)
		{
			return MatchVerifications(verifications, content, VerificationBehaviour.Negative);
		}

		private List<VerificationItem> MatchVerifications(List<VerificationItem> verifications, string content, VerificationBehaviour behaviour)
		{
			var matchedItems = new List<VerificationItem>();

			foreach (VerificationItem item in verifications)
			{
				var simpleLogger = new SimpleLogger();

				LogItem(simpleLogger, behaviour, item);
				string regex = item.Regex;

				if (!string.IsNullOrEmpty(regex))
				{
					regex = _variables.ReplaceVariablesIn(regex);
					item.TransformedRegex = regex;

					LogRegex(simpleLogger, item.Regex, regex);

					try
					{
						bool isMatch = Regex.IsMatch(content, regex, RegexOptions.IgnoreCase);
						item.Success = true;

						if (behaviour == VerificationBehaviour.Positive && isMatch == false)
						{
							item.Success = false;
							LogFail(simpleLogger, behaviour, regex);
						}
						else if (behaviour == VerificationBehaviour.Negative && isMatch == true)
						{
							item.Success = false;
							LogFail(simpleLogger, behaviour, regex);
						}
						else
						{
						LogSuccess(simpleLogger, behaviour, regex);
						}
					}
					catch (ArgumentException e)
					{
						// Invalid regex - ignore.
						item.Success = false;
						LogException(simpleLogger, e);
					}
				}
				else
				{
					LogEmpty(simpleLogger);
				}

				item.Log = simpleLogger.GetLog();
				matchedItems.Add(item);
			}

			return matchedItems;
		}

		private void LogItem(SimpleLogger logger, VerificationBehaviour behaviour, VerificationItem item)
		{
			logger.WriteLine("");
			logger.WriteLine("Verifying {0} item \"{1}\"", behaviour, item.Description);
		}

		private void LogRegex(SimpleLogger logger, string originalRegex, string transformedRegex)
		{
			logger.WriteLine("  - Original regex: {0}", originalRegex);
			logger.WriteLine("  - Regex with variables transformed: {0}", transformedRegex);
		}

		private void LogSuccess(SimpleLogger logger, VerificationBehaviour behaviour, string verifyRegex)
		{
			logger.WriteLine("  - {0} verification successful: the regex \"{1}\" matched.", behaviour, verifyRegex);
		}

		private void LogFail(SimpleLogger logger, VerificationBehaviour behaviour, string verifyRegex)
		{
			logger.WriteLine("  - {0} verification failed: the regex \"{1}\" did not match.", behaviour, verifyRegex);			
		}

		private void LogException(SimpleLogger logger, Exception e)
		{
			logger.WriteLine(" - Invalid regex: {0}", e.Message);
		}

		private void LogEmpty(SimpleLogger logger)
		{
			logger.WriteLine("  - Skipping as the regex was empty.");
		}
	}
}