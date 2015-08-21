using Syringe.Core;
using Syringe.Web.Models;

namespace Syringe.Web.ModelBuilders
{
	public interface ITestCaseCoreModelBuilder
	{
		Case Build(TestCaseViewModel testCase);
	}
}