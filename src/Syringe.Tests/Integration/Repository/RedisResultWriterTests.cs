using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Syringe.Core.Domain.Entities;
using Syringe.Core.Domain.Repository;
using Syringe.Core.Results;
using Syringe.Core.Results.Writer;

namespace Syringe.Tests.Integration.Repository
{
	public class RedisResultWriterTests
	{
		[SetUp]
		public void Setup()
		{
			var resultWriter = CreateRedisResultWriterTests();
			resultWriter.RedisTestCaseResultClient.DeleteAll();
		}

		private RedisResultWriter CreateRedisResultWriterTests()
		{
			return new RedisResultWriter();	
		}

		[Test]
		public void Write_should_store_test_result()
		{
			// Arrange
			var testCaseResult = new TestCaseResult();
			var resultWriter = CreateRedisResultWriterTests();

			// Act
			resultWriter.Write(testCaseResult);

			// Assert
			IList<TestCaseResult> allItems = resultWriter.RedisTestCaseResultClient.GetAll();

			Assert.That(allItems.Count(), Is.EqualTo(1));
		}
	}
}