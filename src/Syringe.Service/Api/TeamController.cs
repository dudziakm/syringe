using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace Syringe.Service.Api
{
    public class TeamController : ApiController
    {
        [Route("api/team/list")]
        [HttpGet]
        public IEnumerable<string> List()
        {
            return new []
            {
                "Spitfire",
                "Loki",
            };
        }
    }
}