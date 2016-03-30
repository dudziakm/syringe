using System;
using System.Collections.Generic;
using Syringe.Core.Security;

namespace Syringe.Web.Models
{
	public interface IRunViewModel
	{
		void Run(IUserContext userContext, string fileName);
	    void RunTest(IUserContext userContext, string fileName, int index);

        IEnumerable<RunningTestViewModel> Tests { get; }
		int CurrentTaskPosition { get; }
		string FileName { get; }
	    string SignalRUrl { get; }
	}
}