using System;
using System.Collections.Generic;
using Syringe.Core.TestCases;

namespace Syringe.Core.Repositories
{
    public interface ICaseRepository
    {
        IEnumerable<string> ListCasesForTeam(string teamName);
        CaseCollection GetTestCaseCollection(string filename, string teamName);
        Case GetTestCase(string filename, string teamName, int index);
        bool SaveTestCase(Case testCase, string teamName);
        bool CreateTestCase(Case testCase, string teamName);
        bool DeleteTestCase(int index, string fileName, string teamName);
        bool CreateTestFile(CaseCollection caseCollection, string teamName);
        bool UpdateTestFile(CaseCollection caseCollection, string teamName);
        string GetXmlTestCaseCollection(string filename, string teamName);
    }
}