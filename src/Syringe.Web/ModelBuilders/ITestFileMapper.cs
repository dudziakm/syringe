using System.Collections.Generic;
using Syringe.Core.Tests;
using Syringe.Web.Models;

namespace Syringe.Web.ModelBuilders
{
    public interface ITestFileMapper
    {
        TestViewModel BuildViewModel(Test test);
        IEnumerable<TestViewModel> BuildTestCases(IEnumerable<Test> tests);
        Test BuildCoreModel(TestViewModel testModel);
    }
}