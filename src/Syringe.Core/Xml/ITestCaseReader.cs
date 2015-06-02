using System;
using System.IO;

namespace Syringe.Core.Xml
{
	public interface ITestCaseReader
    {
		CaseCollection Read(TextReader textReader);
    }
}