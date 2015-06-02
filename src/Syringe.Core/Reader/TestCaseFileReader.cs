using System;
using System.IO;

namespace Syringe.Core.Reader
{
    public class TestCaseFileReader : ITestCaseFileReader
    {
        private readonly ITestCaseDirectory _directory;

        public TestCaseFileReader(ITestCaseDirectory directory)
        {
            _directory = directory;
        }

        public TestCaseFileReader() : this(new TestCaseDirectory()) { }

        public TextReader Get(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException(file, "File not specified");
            }

            var filePath = _directory.GetFullPath(file);

            return new StreamReader(filePath);
        }
    }
}