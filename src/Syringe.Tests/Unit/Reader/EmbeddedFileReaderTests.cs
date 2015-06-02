using System;
using NUnit.Framework;
using Syringe.Core.Reader;

namespace Syringe.Tests.Unit.Reader
{
    [TestFixture]
    public class EmbeddedFileReaderTests
    {
        [TestCase(null)]
        [TestCase("")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Get_should_throw_exception_if_file_is_null_or_empty(string file)
        {
            var embededFileReader = new TestCaseFileReader(new TestCaseDirectory());
            embededFileReader.Get(file);
        }
    }
}
