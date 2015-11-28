﻿using System;
using System.Collections.Generic;
using Syringe.Core.Http;
using Syringe.Core.TestCases;

namespace Syringe.Service.Api.Hubs
{
	public interface ITaskMonitorHubClient
	{
		void OnTaskCompleted(CompletedTaskInfo taskInfo);
	}

	public class CompletedTaskInfo
	{
		public string ActualUrl { get; set; }
		public Guid ResultId { get; set; }
		public bool Success { get; set; }
		public HttpRequestInfo HttpRequestInfo { get; set; }
		public Guid CaseId { get; set; }
		public string ExceptionMessage { get; set; }
        public List<VerificationItem> Verifications { get; set; }    
	}
}