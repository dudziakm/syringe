using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Syringe.Core.Xml;

namespace Syringe.Core.Runner
{
	internal class VerificationMatcher
	{
		private readonly VariableManager _variableManager;

		internal enum VerificationBehaviour
		{
			Negative,
			Positive
		}

		public VerificationMatcher(VariableManager variableManager)
		{
			_variableManager = variableManager;
		}

		public List<RegexItem> MatchPositiveVerifications(List<RegexItem> verifications, string content)
		{
			return MatchVerifications(verifications, content, VerificationBehaviour.Positive);
		}

		public List<RegexItem> MatchNegativeVerifications(List<RegexItem> verifications, string content)
		{
			return MatchVerifications(verifications, content, VerificationBehaviour.Negative);
		}

		private List<RegexItem> MatchVerifications(List<RegexItem> verifications, string content, VerificationBehaviour behaviour)
		{
			var matchedItems = new List<RegexItem>();

			foreach (RegexItem regexItem in verifications)
			{
				Console.WriteLine("Verifying {0} {1}", behaviour, regexItem.Description);
				Console.WriteLine("---------");

				string verifyRegex = regexItem.Regex;

				if (!string.IsNullOrEmpty(verifyRegex))
				{
					verifyRegex = _variableManager.ReplaceVariablesIn(verifyRegex);

					Console.WriteLine("  - Original regex: {0}", regexItem.Regex);
					Console.WriteLine("  - Transformed regex: {0}", verifyRegex);

					try
					{
						bool isMatch = Regex.IsMatch(content, verifyRegex);
						regexItem.Success = true;

						if (behaviour == VerificationBehaviour.Positive && isMatch == false)
						{
							regexItem.Success = false;
							Console.WriteLine("Positive verification failed: {0} - {1}", regexItem.Description, verifyRegex);
						}
						else if (behaviour == VerificationBehaviour.Negative && isMatch == true)
						{
							regexItem.Success = false;
							Console.WriteLine("Negative verification failed: {0} - {1}", regexItem.Description, verifyRegex);
						}
					}
					catch (ArgumentException e)
					{
						// Invalid regex - ignore.
						regexItem.Success = false;
						Console.WriteLine(" - Invalid regex: {0}", e.Message);
					}
				}
				else
				{
					Console.WriteLine("  - Skipping as the regex was empty.");
				}

				matchedItems.Add(regexItem);
			}

			return matchedItems;
		}
	}
}