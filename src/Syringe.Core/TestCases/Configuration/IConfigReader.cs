using System.IO;

namespace Syringe.Core.TestCases.Configuration
{
	public interface IConfigReader
	{
		Config Read(TextReader textReader);
	}
}