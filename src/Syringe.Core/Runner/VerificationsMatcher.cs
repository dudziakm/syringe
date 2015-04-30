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
				Log.Information("Verifying {0} {1}", behaviour, item.Description);
				Log.Information("---------");

				string verifyRegex = item.Regex;

				if (!string.IsNullOrEmpty(verifyRegex))
				{
					verifyRegex = _variables.ReplaceVariablesIn(verifyRegex);

					Log.Information("  - Original regex: {0}", item.Regex);
					Log.Information("  - Transformed regex: {0}", verifyRegex);

					try
					{
						bool isMatch = Regex.IsMatch(content, verifyRegex, RegexOptions.IgnoreCase);
						item.Success = true;

						if (behaviour == VerificationBehaviour.Positive && isMatch == false)
						{
							item.Success = false;
							Log.Information("Positive verification failed: {0} - {1}", item.Description, verifyRegex);
						}
						else if (behaviour == VerificationBehaviour.Negative && isMatch == true)
						{
							item.Success = false;
							Log.Information("Negative verification failed: {0} - {1}", item.Description, verifyRegex);
						}
					}
					catch (ArgumentException e)
					{
						// Invalid regex - ignore.
						item.Success = false;
						Log.Information(" - Invalid regex: {0}", e.Message);
					}
				}
				else
				{
					Log.Information("  - Skipping as the regex was empty.");
				}

				matchedItems.Add(item);
			}

			return matchedItems;
		}
	}
}