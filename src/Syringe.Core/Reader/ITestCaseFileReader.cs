using System.IO;
using System.Reflection;

namespace Syringe.Core.Reader
{
    public interface ITestCaseFileReader
    {
        TextReader Get(string file);
    }
}