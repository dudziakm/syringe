using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syringe.Core.Results;

namespace Syringe.Core.ResultWriter
{
	public interface IResultWriter
	{
		void WriteHeader(string format, params object[] args);
		void Write(TestCaseResult result);
	}
}
