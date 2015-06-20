using System;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using Syringe.Core.Domain.Entities;

namespace Syringe.Core.Results.Writer
{
	public class RedisResultWriter : IResultWriter
	{
		internal RedisClient RedisClient { get; set; }
		internal RedisTypedClient<TestCaseResult> RedisTestCaseResultClient { get; set; }

		public RedisResultWriter()
			: this("localhost", 6379)
		{
		}

		public RedisResultWriter(string host, int port)
		{
			if (host == null)
				throw new ArgumentNullException("host");

			if (port < 1)
				throw new ArgumentException("port is is invalid", "port");

			RedisClient = new RedisClient(host, port);
			RedisTestCaseResultClient = new RedisTypedClient<TestCaseResult>(RedisClient);
		}

		public void WriteHeader(string format, params object[] args)
		{
			throw new System.NotImplementedException();
		}

		public void Write(TestCaseResult result)
		{
			RedisTestCaseResultClient.Store(result);
		}
	}
}