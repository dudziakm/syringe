using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using Syringe.Core;
using Syringe.Core.Xml;
using Syringe.Service.Configuration;

namespace Syringe.Service.Api
{
    public class CasesController : ApiController
    {
		private readonly IConfiguration _configuration;

		public CasesController() : this(new Config())
		{
		}

		internal CasesController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		// TODO: Tests

		[Route("api/cases/ListForTeam")]
		[HttpGet]
		public IEnumerable<string> ListForTeam(string teamName)
		{
			string fullPath = Path.Combine(_configuration.TestCasesBaseDirectory, teamName);

			foreach (string file in Directory.EnumerateFiles(fullPath))
			{
				var fileInfo = new FileInfo(file);
				yield return fileInfo.Name;
			}
		}

		[Route("api/cases/GetByFilename")]
        [HttpGet]
		public CaseCollection GetByFilename(string filename, string teamName)
        {
			string fullPath = Path.Combine(_configuration.TestCasesBaseDirectory, teamName, filename);
			string xml = File.ReadAllText(fullPath);

	        using (var stringReader = new StringReader(xml))
	        {
		        var testCaseReader = new TestCaseReader(stringReader);
		        return testCaseReader.Read();
	        }
        }
    }
}