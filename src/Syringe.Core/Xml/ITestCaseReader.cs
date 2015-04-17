using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Syringe.Core.Xml.LegacyConverter
{
    public interface ITestCaseReader
    {
        TestCaseCollection Read(TextReader textReader);
    }
}