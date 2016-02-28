using System.Collections.Generic;
using Syringe.Core.TestCases;
using Syringe.Web.Models;

namespace Syringe.Web.ModelBuilders
{
    public interface ITestCaseMapper
    {
        TestCaseViewModel BuildViewModel(Case testCase);
        IEnumerable<TestCaseViewModel> BuildTestCases(IEnumerable<Case> cases);
        Case BuildCoreModel(TestCaseViewModel testCase);
    }
}