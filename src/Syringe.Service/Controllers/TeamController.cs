using System.Collections.Generic;
using System.Web.Http;

namespace Syringe.Service.Controllers
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

		// AddUser
		// DeactivateUser
		// GetTeamConfig
		// UpdateTeamConfig
		// GetUserProfile
		// UpdateUserProfile
		// GetUserConfig
		// UpdateUserConfig
    }
}