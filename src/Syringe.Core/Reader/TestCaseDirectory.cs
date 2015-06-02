using System;
using System.IO;

namespace Syringe.Core.Reader
{
    public class TestCaseDirectory : ITestCaseDirectory
    {
        public string GetFullPath(string filename)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = Path.Combine(path, "XmlExamples\\");

            if (string.IsNullOrEmpty(path))
            {
                throw new NullReferenceException("Directory not found" + path);
            }

            var fullPath = string.Format("{0}{1}{2}", path, filename, ".xml");

            return fullPath;
        }
    }
}