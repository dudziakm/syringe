using RestSharp;

namespace Syringe.Core.Domain.Providers
{
    public class TeamcityUserRequest
    {
        private const string resourceUrl = "/guestAuth/app/rest/users/{0}";

        public RestRequest GetRequest(string userName)
        {
            var request = new RestRequest(Method.GET)
                          {
                              Resource = string.Format(resourceUrl, userName),
                              RootElement = "user"
                          };

            request.AddHeader("Accept", "application/json");

            return request;
        }
    }
}
