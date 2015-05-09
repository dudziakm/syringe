using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bender.Collections;
using Syringe.Core.Logging;

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
				LogItem(behaviour, item);
				string regex = item.Regex;

				if (!string.IsNullOrEmpty(regex))
				{
					regex = _variables.ReplaceVariablesIn(regex);
					item.TransformedRegex = regex;

					LogRegex(item.Regex, regex);

					try
					{
						bool isMatch = Regex.IsMatch(content, regex, RegexOptions.IgnoreCase);
						item.Success = true;

						if (behaviour == VerificationBehaviour.Positive && isMatch == false)
						{
							item.Success = false;
							LogFail(behaviour, item, regex);
						}
						else if (behaviour == VerificationBehaviour.Negative && isMatch == true)
						{
							item.Success = false;
							LogFail(behaviour, item, regex);
						}
					}
					catch (ArgumentException e)
					{
						// Invalid regex - ignore.
						item.Success = false;
						LogException(e);
					}
				}
				else
				{
					LogEmpty();
				}

				matchedItems.Add(item);
			}

			return matchedItems;
		}

		private void LogItem(VerificationBehaviour behaviour, VerificationItem item)
		{
			Log.Information("---------------------------");
			Log.Information("Verifying {0} [{1}]", behaviour, item.Description);
			Log.Information("---------------------------");
		}

		private void LogRegex(string originalRegex, string transformedRegex)
		{
			Log.Information("  - Original regex: {0}", originalRegex);
			Log.Information("  - Transformed regex: {0}", transformedRegex);
		}

		private void LogFail(VerificationBehaviour behaviour, VerificationItem item, string verifyRegex)
		{
			Log.Information("{0} verification failed: {1} - {2}", behaviour, item.Description, verifyRegex);			
		}

		private void LogException(Exception e)
		{
			Log.Information(" - Invalid regex: {0}", e.Message);
		}

		private void LogEmpty()
		{
			Log.Information("  - Skipping as the regex was empty.");
		}
	}
}