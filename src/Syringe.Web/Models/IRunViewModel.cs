using System;
using System.Collections.Generic;
using Syringe.Core.Security;

namespace Syringe.Web.Models
{
	public interface IRunViewModel
	{
		void Run(IUserContext userContext, string fileName);
	    void RunTest(IUserContext userContext, string fileName, Guid testCaseId);

        IEnumerable<RunningTestCaseViewModel> TestCases { get; }
		int CurrentTaskId { get; }
		string FileName { get; }
	    string SignalRUrl { get; }
	}
}