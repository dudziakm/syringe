using System.IO;
using Syringe.Core.Tests;

namespace Syringe.Core.Xml.Reader
{
	public interface ITestFileReader
    {
		TestFile Read(TextReader textReader);
    }
}