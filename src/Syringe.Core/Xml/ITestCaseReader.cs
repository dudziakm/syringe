using System.IO;

namespace Syringe.Core.Xml
{
    public interface ITestCaseReader
    {
        TestCaseCollection Read(TextReader textReader);
    }
}