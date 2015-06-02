using System;
using System.IO;
using System.Linq;
using Syringe.Core.Reader;
using Syringe.Core.Xml;

namespace Syringe.Core.Repositories
{
    public class CaseRepository : ICaseRepository
    {
        private readonly ITestCaseFileReader _testCaseFileReader;

        public CaseRepository(ITestCaseFileReader testCaseFileReader)
        {
            _testCaseFileReader = testCaseFileReader;
        }

        public CaseRepository() : this(new TestCaseFileReader()) { }

        public Case GetTestCase(string filename, int caseId)
        {
            TextReader textReader = _testCaseFileReader.Get(filename);

            using (var reader = new TestCaseReader(textReader))
            {
                CaseCollection collection = reader.Read();

                Case testCase = collection.TestCases.First(x => x.Id == caseId);

                if (testCase == null)
                {
                    throw new NullReferenceException("Could not find specified Test Case:" + caseId);
                }

                return testCase;
            }
        }
    }
}