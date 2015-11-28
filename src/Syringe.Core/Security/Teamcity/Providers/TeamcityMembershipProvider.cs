﻿using System;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;

namespace Syringe.Core.Security.Teamcity.Providers
{
    public class TeamLookupProvider : ITeamLookupProvider
    {
        private readonly IRestClient _restClient;
        private const string BaseUrl = "http://teamcity.yourdomain.com:8500/";

        public TeamLookupProvider()
        {
            _restClient = new RestClient();
        }

        public TeamLookupProvider(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public string GetTeamNameForUser(string user)
        {
            var result = GetUserForUserName(user);

            var groupName = result.Groups.Group.FirstOrDefault(x => x.Key.Contains("TEAM"));

            return groupName.Name;
        }

        public User GetUserForUserName(string user)
        {
            var teamCityRequest = new TeamcityUserRequest();
            var request = teamCityRequest.GetRequest(user);

            return Execute(request);
        }

        public User Execute(IRestRequest request)  
        {
            var client = _restClient;
            client.BaseUrl = new Uri(BaseUrl);

            IRestResponse response = client.Execute(request);		   

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var teamcityExpception = new ApplicationException(message, response.ErrorException);
                throw teamcityExpception;
            }

            return JsonConvert.DeserializeObject<User>(response.Content);
        }
    }
}