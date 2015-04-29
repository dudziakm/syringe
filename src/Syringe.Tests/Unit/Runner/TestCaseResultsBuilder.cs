using System;
using System.Collections.Generic;
using Syringe.Core;
using Syringe.Core.Results;

namespace Syringe.Tests.Unit.Runner
{
	internal class TestCaseResultsBuilder
	{
		private TestCaseResult _currentTestCaseResult;
		private readonly List<TestCaseResult> _testCases;

		public TestCaseResultsBuilder()
		{
			_testCases = new List<TestCaseResult>();
		}

		public TestCaseResultsBuilder New()
		{
			_currentTestCaseResult = new TestCaseResult();
			return this;
		}

		public TestCaseResultsBuilder WithSuccess()
		{
			_currentTestCaseResult.VerifyResponseCodeSuccess = true;
			return this;
		}

		public TestCaseResultsBuilder WithFail()
		{
			_currentTestCaseResult.VerifyResponseCodeSuccess = false;
			return this;
		}

		public TestCaseResultsBuilder AddPositiveVerify(bool success = true)
		{
			_currentTestCaseResult.VerifyPositiveResults.Add(new RegexItem("item " + DateTime.Now, "regex") { Success = success });
			return this;
		}

		public TestCaseResultsBuilder AddNegativeVerify(bool success = true)
		{
			_currentTestCaseResult.VerifyNegativeResults.Add(new RegexItem("item " + DateTime.Now, "regex") { Success = success });
			return this;
		}

		public TestCaseResultsBuilder Add()
		{
			_testCases.Add(_currentTestCaseResult);
			return this;
		}

		public List<TestCaseResult> GetCollection()
		{
			return _testCases;
		}
	}
}