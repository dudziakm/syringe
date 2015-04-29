using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

		public List<RegexItem> MatchPositive(List<RegexItem> verifications, string content)
		{
			return MatchVerifications(verifications, content, VerificationBehaviour.Positive);
		}

		public List<RegexItem> MatchNegative(List<RegexItem> verifications, string content)
		{
			return MatchVerifications(verifications, content, VerificationBehaviour.Negative);
		}

		private List<RegexItem> MatchVerifications(List<RegexItem> verifications, string content, VerificationBehaviour behaviour)
		{
			var matchedItems = new List<RegexItem>();

			foreach (RegexItem regexItem in verifications)
			{
				Log.Information("Verifying {0} {1}", behaviour, regexItem.Description);
				Log.Information("---------");

				string verifyRegex = regexItem.Regex;

				if (!string.IsNullOrEmpty(verifyRegex))
				{
					verifyRegex = _variables.ReplaceVariablesIn(verifyRegex);

					Log.Information("  - Original regex: {0}", regexItem.Regex);
					Log.Information("  - Transformed regex: {0}", verifyRegex);

					try
					{
						bool isMatch = Regex.IsMatch(content, verifyRegex, RegexOptions.IgnoreCase);
						regexItem.Success = true;

						if (behaviour == VerificationBehaviour.Positive && isMatch == false)
						{
							regexItem.Success = false;
							Log.Information("Positive verification failed: {0} - {1}", regexItem.Description, verifyRegex);
						}
						else if (behaviour == VerificationBehaviour.Negative && isMatch == true)
						{
							regexItem.Success = false;
							Log.Information("Negative verification failed: {0} - {1}", regexItem.Description, verifyRegex);
						}
					}
					catch (ArgumentException e)
					{
						// Invalid regex - ignore.
						regexItem.Success = false;
						Log.Information(" - Invalid regex: {0}", e.Message);
					}
				}
				else
				{
					Log.Information("  - Skipping as the regex was empty.");
				}

				matchedItems.Add(regexItem);
			}

			return matchedItems;
		}
	}
}