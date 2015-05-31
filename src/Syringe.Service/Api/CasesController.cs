using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using Syringe.Core;
using Syringe.Core.Xml;

namespace Syringe.Service.Api
{
    public class CasesController : ApiController
    {
		// TODO: Tests
		[Route("api/cases/GetByFilename")]
        [HttpGet]
		public CaseCollection GetByFilename(string filename)
        {
			string xml = File.ReadAllText(Path.Combine(@"d:\syringe\", filename));

	        using (var stringReader = new StringReader(xml))
	        {
		        var testCaseReader = new TestCaseReader(stringReader);
		        return testCaseReader.Read();
	        }
        }
    }
}