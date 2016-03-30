using System;
using System.Collections.Generic;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;

namespace Syringe.Tests.Unit.Runner
{
	internal class TestCaseResultsBuilder
	{
		private readonly List<TestResult> _testCases;
		private TestResult _currentTestResult;

		public TestCaseResultsBuilder()
		{
			_testCases = new List<TestResult>();
		}

		public TestCaseResultsBuilder New()
		{
			_currentTestResult = new TestResult();
			return this;
		}

		public TestCaseResultsBuilder WithSuccess()
		{
			_currentTestResult.ResponseCodeSuccess = true;
			return this;
		}

		public TestCaseResultsBuilder WithFail()
		{
			_currentTestResult.ResponseCodeSuccess = false;
			return this;
		}

		public TestCaseResultsBuilder AddPositiveVerify(bool success = true)
		{
			_currentTestResult.PositiveAssertionResults.Add(new Assertion("item " + DateTime.Now, "regex",
				AssertionType.Positive) {Success = success});
			return this;
		}

		public TestCaseResultsBuilder AddNegativeVerify(bool success = true)
		{
			_currentTestResult.NegativeAssertionResults.Add(new Assertion("item " + DateTime.Now, "regex",
				AssertionType.Negative) {Success = success});
			return this;
		}

		public TestCaseResultsBuilder Add()
		{
			_testCases.Add(_currentTestResult);
			return this;
		}

		public List<TestResult> GetCollection()
		{
			return _testCases;
		}

		public TestCaseResultsBuilder WithMessage(string message)
		{
			_currentTestResult.Message = message;
			return this;
		}
	}
}